using UnityEngine;
using System.Collections.Generic;
using RAIN.Core;
using RAIN.Memory;
using RAIN.Motion;
using RAIN.Navigation;

public class EnemyOnServer : Enemy, CasterOnServer {
	// AI
	protected AIRig aiRig;
	protected RAINMemory aiMemory;
	protected RAINMotor aiMotor;
	protected BasicNavigator aiNavigator;
	protected DangerDetector dangerDetector;

	protected List<RAIN.Perception.Sensors.RAINSensor> sensors;
	protected RAIN.Perception.Sensors.RAINSensor visualSensor;
	protected RAIN.Perception.Sensors.RAINSensor audioSensor;
	
	// RPC names
	private string rpcState = "M";

	private int hpAggroThreshold;
	private int lastHPSent;
	
	// Threat from each player
	public Dictionary<Entity, int> entityToThreat;
	private List<Entity> entitiesToRemove;
	
	// Awake
	protected override void Awake() {
		base.Awake();

		// Controller
		controller = new AIController(this);

		hpAggroThreshold = Config.instance.hpAggroThreshold;
		entityToThreat = new Dictionary<Entity, int>();
		entitiesToRemove = new List<Entity>();

		// Danger detector
		var dangerDetectorChild = InstantiateChild(Config.instance.dangerDetector);
		dangerDetector = dangerDetectorChild.GetComponent<DangerDetector>();
		dangerDetectorChild.GetComponent<SphereCollider>().radius = characterController.height * 5f;
		dangerDetectorChild.gameObject.SetActive(false);

		// Events
		onDeath += OnDeath;
	}
	
	// Start
	void Start() {
		// Cached components
		aiRig = GetComponentInChildren<AIRig>();
		aiMemory = aiRig.AI.WorkingMemory;
		aiMotor = aiRig.AI.Motor;
		aiNavigator = (BasicNavigator)aiRig.AI.Navigator;

		// Sensors
		sensors = new List<RAIN.Perception.Sensors.RAINSensor>(aiRig.AI.Senses.Sensors);
		visualSensor = aiRig.AI.Senses.GetSensor("Visual Sensor");
		audioSensor = aiRig.AI.Senses.GetSensor("Audio Sensor");

		// Repeated calls
		InvokeRepeating("SendToClients", 0.001f, 1.0f / uLink.Network.sendRate);
		InvokeRepeating("React", 0.001f, Config.instance.enemyReactionTime);
	}
	
	// FixedUpdate
	void FixedUpdate() {
		// Skill effects
		UpdateSkillEffects();
		
		// Movement
		UpdateMovement();

		// Energy
		UpdateEnergyOnServer();

		// Skill casts
		UpdateStartCast();

		// Map boundaries
		StayInMapBoundaries();
	}

	// React
	void React() {
		if(!isAlive)
			return;

		// Visual sensor
		var aspectsDetected = aiRig.AI.Senses.Match(sensors, "Player");
		foreach(var aspect in aspectsDetected) {
			var player = aspect.Entity.Form.GetComponent<Player>();
			if(!player.isAlive)
				continue;
			
			if(!entityToThreat.ContainsKey(player))
				entityToThreat[player] = 0;
			else
				entityToThreat[player]++;
		}

		// Threat
		var oldTargetId = targetId;
		target = FindTargetWithHighestThreat();
		
		// Target changed?
		if(targetId != oldTargetId) {
			LogSpam("New target: '" + target + "'");
			networkView.RPC("ClientSetTarget", uLink.RPCMode.Server, targetId);
			SetTarget(targetId);
			
			if(hasTarget) {
				// Activate danger detector
				if(!dangerDetector.gameObject.activeSelf)
					dangerDetector.gameObject.SetActive(true);
				
				aiMemory.SetItem<bool>("hasTarget", true);
			} else {
				// Deactivate danger detector
				if(dangerDetector.gameObject.activeSelf)
					dangerDetector.gameObject.SetActive(false);
				
				aiMemory.SetItem<bool>("hasTarget", false);
			}
		}
		
		// Blocking
		UpdateDangerDetection();
	}

	// UpdateDangerDetection
	void UpdateDangerDetection() {
		var dangerous = dangerDetector.detectedDanger;

		if(dangerous && canBlock) {
			// Start blocking
			networkView.RPC("StartBlock", uLink.RPCMode.OthersExceptOwner);
			StartBlock();
		} else if(!dangerous && blocking) {
			// End blocking
			networkView.RPC("EndBlock", uLink.RPCMode.OthersExceptOwner);
			EndBlock();
		}
	}
	
	// UpdateMovement
	void UpdateMovement() {
		if(cantMove) {
			aiMotor.DefaultSpeed = 0f;
			aiMotor.Stop();
		} else {
			// Found a target?
			if(target != null) {
				// Target location
				aiMemory.SetItem<Vector3>("targetLocation", target.position);

				// Set move speed to maximum speed
				aiMotor.DefaultSpeed = moveSpeed;
			} else {
				// Set move speed to patrol speed
				aiMotor.DefaultSpeed = moveSpeed * Config.instance.patrolSpeedModifier;
			}
		}
	}

	// SendToClients
	void SendToClients() {
		if(!isAlive && lastHPSent == 0)
			return;

		// Position
		networkView.UnreliableRPC(rpcState, uLink.RPCMode.Others,
			myTransform.position,
			aiMotor.IsAtMoveTarget ? Cache.vector3Zero : characterController.velocity,
			(ushort)(charGraphics.eulerAngles.y * Cache.rotationFloatToShort)
		);
		
		// HP
		if(health != lastHPSent) {
			networkView.RPC("SetHP", uLink.RPCMode.Others, (ushort)health);
			
			lastHPSent = health;
			
			// Death
			if(health == 0) {
				// Let others know about the kill
				networkView.RPC("RegisterKill", uLink.RPCMode.All,
					lastHitBy.id,				// Killer
				   	id,							// Victim
				    (short)lastHitBySkill.id	// Skill ID
				);
			}
		}
	}

	// OnCastRestart
	protected override void OnCastRestart() {
		StartCoroutine(
			UpdateEndCast(
				currentSkill.currentStage.castDuration * attackSpeedMultiplier,
				currentSpellId
			)
		);
	}
	
	// FindTargetWithHighestThreat
	Entity FindTargetWithHighestThreat() {
		Entity threatTarget = null;
		int highestThreat = -1;
		
		Entity lowHPTarget = null;
		int lowestHP = int.MaxValue;

		// Clear the list of entities to remove
		entitiesToRemove.Clear();

		Entity entity;
		foreach(var threat in entityToThreat) {
			entity = threat.Key;
			
			if(!entity.isAlive) {
				entitiesToRemove.Add(entity);
				continue;
			}
			
			// Threat
			if(threat.Value > highestThreat) {
				threatTarget = entity;
				highestThreat = threat.Value;
			}
			
			// Lowest HP
			if(entity.health < lowestHP) {
				lowHPTarget = entity;
				lowestHP = entity.health;
			}
		}

		// Remove all dead entities from the threat dictionary
		foreach(var deadEntity in entitiesToRemove) {
			entityToThreat.Remove(deadEntity);
		}
		
		// If someone is close to dying we'll pick him
		if(lowestHP <= hpAggroThreshold)
			return lowHPTarget;
		// Otherwise pick the target with the highest threat
		else
			return threatTarget;
	}

	// OnDeath
	void OnDeath() {
		// Disable AI
		aiRig.enabled = false;

		Invoke("DestroyMyself", Config.instance.enemyRespawnTime);
	}
	
	// DestroyMyself
	void DestroyMyself() {
		uLink.Network.RemoveRPCs(networkView.viewID);
		uLink.Network.Destroy(gameObject);
	}

#region RPCs
	[RPC]
	public void ClientStartCast(byte slotId, uLink.NetworkMessageInfo info) {
		networkView.RPC("StartCast", uLink.RPCMode.OthersExceptOwner, slotId);
	}
	
	[RPC]
	public void ClientAdvanceCast(uLink.NetworkMessageInfo info) {
		networkView.RPC("AdvanceCast", uLink.RPCMode.OthersExceptOwner);
	}
	
	[RPC]
	public void ClientCouldntCast(uLink.NetworkMessageInfo info) {
		networkView.RPC("CouldntCast", uLink.RPCMode.OthersExceptOwner);
	}
	
	[RPC]
	public void ClientEndCast(Vector3 hitPoint, uLink.NetworkMessageInfo info) {
		networkView.RPC("EndCast", uLink.RPCMode.OthersExceptOwner, hitPoint);
	}
	
	[RPC]
	public void ClientInstantCast(byte skillId, Vector3 hitPoint, uLink.NetworkMessageInfo info) {
		networkView.RPC("InstantCast", uLink.RPCMode.OthersExceptOwner, skillId, hitPoint);
	}
	
	[RPC]
	public void ClientStopHolding(uLink.NetworkMessageInfo info) {
		networkView.RPC("StopHolding", uLink.RPCMode.OthersExceptOwner);
	}

	[RPC]
	public void ClientSetTarget(ushort entityId, uLink.NetworkMessageInfo info) {
		networkView.RPC("SetTarget", uLink.RPCMode.OthersExceptOwner, entityId);
	}
#endregion
}
