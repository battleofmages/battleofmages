using UnityEngine;
using System;
using System.Collections.Generic;

public abstract partial class Entity : uLink.MonoBehaviour, PartyMember<Entity> {
	public static Dictionary<ushort, Entity> idToEntity = new Dictionary<ushort, Entity>();

	protected float heightMultiplier = 1f;
	protected bool reliablePosSent;
	protected double lastRespawn;
	protected Vector3 centerOffset;
	protected byte currentWeaponSlotId;

	// Proxy
	protected float proxyInterpolationTime = 0f;
	protected float serverRotationY;
	protected Vector3 interpolationStartPosition;
	
	// Server and client side data
	private string _entityName;
	protected string logPrefix;

	// Guild
	private string _guildName = "";
	private string _guildTag = "";

	[NonSerialized]
	public Transform myTransform;

	[NonSerialized]
	public double ignoreNewPositionEarlierThanTimestamp = -1d;
	
	[NonSerialized]
	public int disableSnappingToNewPosition = 0;

	[NonSerialized]
	public GameObject weaponModel;
	
	[NonSerialized]
	public Collider weaponModelCollider;
	
	[NonSerialized]
	public Collider weaponModelProjectileDeflector;

	[NonSerialized]
	public PlayerStats stats;

	[NonSerialized]
	public Dictionary<Entity, int> hitsByPlayer;

	[NonSerialized]
	public Entity lastHitBy;

	[NonSerialized]
	public Transform charGraphics;
	private Transform _charGraphicsModel;
	
	[NonSerialized]
	public Transform charGraphicsBody;

#region Methods
	// Awake
	protected virtual void Awake() {
		// Init subsystems
		InitComponents();
		InitID();
		InitEvents();
		InitSkillSystem();
		InitTraits();
		InitBlock();

		// Stats
		stats = new PlayerStats();
	}

	// InitComponents
	void InitComponents() {
		myTransform = transform;
		nameLabel = GetComponent<EntityLabel>();
		entityGUI = GetComponent<EntityGUI>();
		characterController = GetComponent<CharacterController>();
		charGraphics = myTransform.FindChild("CharGraphics");
	}

	// Applies damage to the player
	public static void ApplyDamage(Entity entity, SkillInstance skillInstance, int power) {
		// Dead entity or protected by spawn protection
		if(!entity.isAlive || entity.hasSpawnProtection)
			return;
		
		// Caster of the skill
		Entity caster = skillInstance.caster;

		// Get stats of the caster and the player
		PlayerQueueStats casterStats = caster.stats.total;
		PlayerQueueStats playerStats = entity.stats.total;
		
		// Blocked hits
		if(entity.blocking) {
			// Show blocked text
			if(uLink.Network.isClient) {
				if(caster == Player.main)
					Entity.SpawnText(entity, "Blocked", new Color(1.0f, 0.5f, 0.0f, 1.0f));
				else if(entity == Player.main)
					Entity.SpawnText(entity, "Blocked", new Color(1.0f, 0.5f, 0.0f, 1.0f), 0, Config.instance.ownDmgOffset);
			}
			
			// Player blocked a hit, count them
			playerStats.blocks += 1;
			casterStats.blocksTaken += 1;

			// Generate threat
			entity.AddThreat(caster, 0);

			// Player blocked: Outta here.
			return;
		}
		
		// Player didn't block.
		
		// The skill used
		Skill skill = skillInstance.skill;
		Skill.Stage skillStage = skillInstance.skillStage;
		
		// Wake up player if he was sleeping and the skill is a damage skill
		if(power > 0 && entity.slept > 0) {
			entity.EndSleep();
			entity.slept = 0;
		}
		
		// Calculate the actual damage
		int dmg = entity.GetModifiedDamage(power * caster.charStats.attackDmgMultiplier * caster.currentWeapon.damageMultiplier);
		
		//LogManager.General.Log("Dmg: " + dmg + ", Power: " + power);
		
		// Detonate runes
		if(skillStage.isRuneDetonator) {
			entity.DetonateRunes(caster);
		}
		
		// Depending on whether this is the server or client we do different stuff
		if(uLink.Network.isServer) {
			// Apply stagger
			if(skillStage.staggerDuration > 0f)
				entity.networkView.RPC("Stagger", uLink.RPCMode.All, skillStage.staggerDuration);
			
			// CC effects
			if(skillStage.effect != null)
				entity.networkView.RPC("ReceiveSkillEffect", uLink.RPCMode.All, (byte)skillStage.effect.id, caster.id);
			
			// Apply runes
			if(skillStage.runeType != Skill.RuneType.None) {
				byte runeId = (byte)((int)(skillStage.runeType) - 1);
				entity.networkView.RPC("ReceiveRune", uLink.RPCMode.All, runeId);
			}
			
			// Skill damage log
			if(entity.skillDamageReceived.ContainsKey(skill.id)) {
				entity.skillDamageReceived[skill.id] += dmg;
			} else {
				entity.skillDamageReceived.Add(skill.id, dmg);
			}
			
			// How many hits did each player do (to save assists)
			if(entity.hitsByPlayer.ContainsKey(caster)) {
				entity.hitsByPlayer[caster] += 1;
			} else {
				entity.hitsByPlayer.Add(caster, 1);
			}
			
			// The actual application of damage
			entity.health -= dmg;
			
			// Caster scored a hit.
			// Update the total hit counter:
			casterStats.hits += 1;
			playerStats.hitsTaken += 1;
			
			// Life drain
			// TODO: Only drain the actual amount of HP / damage he did
			if(skillStage.lifeDrainRel > 0) {
				int lifeDrain = (int)(dmg * skillStage.lifeDrainRel);
				
				// Life drain needs to be capped at 100%
				if(lifeDrain > dmg)
					lifeDrain = dmg;
				
				casterStats.lifeDrain += lifeDrain;
				playerStats.lifeDrainTaken += lifeDrain;
				caster.health += lifeDrain;
			}

			// Generate threat
			entity.AddThreat(caster, dmg);
		} else if(power != 0) {
			// Client shows dmg number
			if(caster == Player.main)
				Entity.SpawnText(entity, dmg.ToString(), new Color(1.0f, 1.0f, 0.4f, 1.0f));
			else if(entity == Player.main)
				Entity.SpawnText(entity, dmg.ToString(), new Color(1.0f, 0.0f, 0.0f, 1.0f), 0, Config.instance.ownDmgOffset);
			
			// Client shows life drain heal
			if(skillStage.lifeDrainRel > 0 && (caster == Player.main || entity == Player.main)) {
				if(caster == Player.main)
					Entity.SpawnText(caster, "+" + ((int)(dmg * skillStage.lifeDrainRel)), new Color(0.0f, 1.0f, 0.0f, 1.0f), 0, Config.instance.ownDmgOffset);
				else if(entity == Player.main)
					Entity.SpawnText(caster, "+" + ((int)(dmg * skillStage.lifeDrainRel)), new Color(0.2f, 1.0f, 0.2f, 1.0f));
			}
		}
		
		// Update combo counter
		if(caster.comboCounter != null)
			caster.comboCounter.AddHit(dmg);
		
		// Last hit
		entity.lastHitBy = caster;
		entity.lastHitBySkill = skill;
		
		// Update CC stats
		if(skillStage.effect != null) {
			casterStats.cc += 1;
			playerStats.ccTaken += 1;
		}
		
		// Update damage stats
		casterStats.damage += dmg;
		playerStats.damageTaken += dmg;
	}

	// AddThreat
	public void AddThreat(Entity caster, int dmg) {
		if(this is EnemyOnServer) {
			var enemy = this as EnemyOnServer;
			int previousDmg;
			
			if(enemy.entityToThreat.TryGetValue(caster, out previousDmg))
				enemy.entityToThreat[caster] = previousDmg + dmg;
			else
				enemy.entityToThreat[caster] = dmg;
		}
	}
	
	// Movement of proxies
	protected void UpdateProxyMovement() {
		proxyInterpolationTime += Time.deltaTime;

		// Interpolate position
		if(myTransform.position != serverPosition) {
			/*Log("proxy movement " + proxyInterpolationTime);
			Log("server " + serverPosition);
			Log("myself " + myTransform.position);*/

			// TODO: Use Slerp?
			var targetPosition = Vector3.Lerp(interpolationStartPosition, serverPosition, proxyInterpolationTime * Config.instance.proxyInterpolationSpeed);
			var offset = targetPosition - myTransform.position;
			
			if(offset.sqrMagnitude < Config.instance.maxProxyDistanceUntilSnapSqr && collider.enabled && proxyInterpolationTime < 1f) {
				characterController.Move(offset);
			} else {
				Log("Snapped to server position, squared distance: " + offset.sqrMagnitude);
				myTransform.position = targetPosition;
			}
		}
		
		// Fix 0/360 clamping for the interpolation of proxy rotation
		float fromY = charGraphics.eulerAngles.y;
		serverRotationY = FixAngleClamping(fromY, serverRotationY);
		if(fromY != serverRotationY)
			charGraphics.eulerAngles = new Vector3(0, Mathf.Lerp(fromY, serverRotationY, proxyInterpolationTime * Config.rotationInterpolationSpeed), 0);
		
		// Target based movement
//		if(rolling && target != null && rollVectorNonTransformed != Cache.vector3Zero) {
//			charGraphics.rotation = Quaternion.LookRotation(rollVector);
//		} else {
//			charGraphics.eulerAngles = new Vector3(0, Mathf.Lerp(fromY, serverRotationY, Time.deltaTime * proxyInterpolationSpeed), 0);
//		}
	}
	
	// Reset rotation
	protected void ResetRotation() {
		if(myTransform.localRotation != Cache.quaternionIdentity)
			myTransform.localRotation = Cache.quaternionIdentity;
	}
	
	// Stay in map boundaries
	public void StayInMapBoundaries() {
		Vector3 pos = MapManager.StayInMapBoundaries(myTransform.position);
		
		if(pos != myTransform.position)
			myTransform.position = pos;
	}
	
	// Spawns an explosion using a Collision
	public void SpawnExplosion(Transform explosionPrefab, Collision collision, SkillInstance copyInst) {
		// Rotate the object so that the y-axis faces along the normal of the surface
		ContactPoint contact = collision.contacts[0];
		Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
		Vector3 pos = contact.point;
		
		SpawnExplosion(explosionPrefab, pos, rot, copyInst);
	}
	
	// Spawns an explosion using position and rotation
	public void SpawnExplosion(Transform explosionPrefab, Vector3 pos, Quaternion rot, SkillInstance copyInst) {
		// Spawn it
		Transform expl = (Transform)Instantiate(explosionPrefab, pos, rot);
		expl.parent = Entity.skillInstancesRoot;
		
		if(uLink.Network.isServer && expl.audio != null) {
			Destroy(expl.audio);
		}
		
		if(expl != null && gameObject != null) {
			Entity.MoveToLayer(expl, gameObject.layer);
			
			// Set caster
			SkillInstance inst = expl.GetComponent<SkillInstance>();
			
			if(inst == null)
				LogManager.General.LogWarning("You forgot to give the Explosion an Explosion script component");
			
			// Set skill instance info
			inst.caster = this;
			inst.skill = copyInst.skill;
			inst.skillStage = copyInst.skillStage;
		}
	}
	
	// Returns the applied damage
	public int GetModifiedDamage(float dmg) {
		return (int)(dmg * charStats.defenseDmgMultiplier);
	}

	// Fix angle clamping
	public static float FixAngleClamping(float fromY, float toY) {
		if(toY - fromY > 180)
			return toY - 360;
		else if(fromY - toY > 180)
			return toY + 360;
		
		return toY;
	}
	
	// MoveToLayer
	public static void MoveToLayer(Transform root, int layer) {
		root.gameObject.layer = layer;
		foreach(Transform child in root)
			MoveToLayer(child, layer);
	}
	
	// SpawnText
	public static void SpawnText(Entity target, string text, Color col, int customOffsetX = 0, int customOffsetY = 0) {
		var prefab = Config.instance.damageNumber;
		
		int offsetX = UnityEngine.Random.Range(-20, 20) + customOffsetX;
		int offsetY = UnityEngine.Random.Range(30, 70) + customOffsetY;
		
		GameObject damageNumberObj = (GameObject)UnityEngine.Object.Instantiate(prefab, Cache.vector3Zero, Cache.quaternionIdentity);
		damageNumberObj.guiText.text = text;
		damageNumberObj.guiText.pixelOffset = new Vector2(offsetX, offsetY);
		damageNumberObj.guiText.material.color = col;
		var damageNumber = damageNumberObj.GetComponent<DamageNumber>();
		damageNumber.target = target;
		
		GameObject dmgNumShadow = (GameObject)UnityEngine.Object.Instantiate(prefab, Cache.vector3Zero, Cache.quaternionIdentity);
		dmgNumShadow.guiText.text = text;
		dmgNumShadow.guiText.pixelOffset = new Vector2(offsetX + 1, offsetY - 1);
		DamageNumber shadowFD = dmgNumShadow.GetComponent<DamageNumber>();
		shadowFD.target = target;
		shadowFD.guiText.material.color = Color.black;
	}

	// InstantiateChild
	public Transform InstantiateChild(GameObject prefab, Transform parent = null) {
		var childTransform = ((GameObject)UnityEngine.Object.Instantiate(prefab)).transform;
		
		// Move to same layer
		Entity.MoveToLayer(childTransform, layer);
		
		// Set parent
		if(parent == null)
			childTransform.parent = myTransform;
		else
			childTransform.parent = parent;
		
		childTransform.localPosition = Cache.vector3Zero;
		childTransform.localRotation = Cache.quaternionIdentity;
		
		return childTransform;
	}

	// GetLookRotation
	public Quaternion GetLookRotation(Entity entity) {
		return Quaternion.LookRotation(myTransform.position - entity.position);
	}

	// GetLookRotation
	public Quaternion GetLookRotation(Transform otherTransform) {
		return Quaternion.LookRotation(myTransform.position - otherTransform.position);
	}

	// GetLookRotation
	public Quaternion GetLookRotation(MonoBehaviour monoBehaviour) {
		return Quaternion.LookRotation(myTransform.position - monoBehaviour.transform.position);
	}

	// UpdateName
	private void UpdateName() {
		logPrefix = "[" + _entityName + " #" + id + "] ";
		
		if(this == Player.main)
			myTransform.name = _entityName + " #" + id + ", Main)";
		else
			myTransform.name = _entityName + " #" + id;
	}
	
	// UpdateNameLabelText
	void UpdateNameLabelText() {
		if(nameLabel == null)
			return;
		
		if(guildTag != "") {
			nameLabel.text = "[" + guildTag + "] " + name;
		} else {
			nameLabel.text = name;
		}
	}
	
	// ToString
	public override string ToString() {
		return name;
	}
#endregion

#region Virtual
	// --------------------------------------------------------------------------------
	// Virtual
	// --------------------------------------------------------------------------------
	
	// ResendDataToPlayer
	public virtual void ResendDataToPlayer(uLink.NetworkPlayer player) {
		networkView.RPC("SetId", player, id);
		networkView.RPC("SetHP", player, (ushort)health);
		networkView.RPC("SetEnergy", player, energy);
		
		if(hasTarget)
			networkView.RPC("SetTarget", player, target.id);
	}
	
	// GetHitPoint
	protected virtual Vector3 GetHitPoint() {
		return Cache.vector3Zero;
	}

	// On network instantiation
	protected virtual void uLink_OnNetworkInstantiate(uLink.NetworkMessageInfo info) {
		InitNetworkView();
	}
#endregion
	
#region PartyMember
	// SetParty
	public void SetParty(Party<Entity> pty) {
		party = (GameServerParty)pty;
	}
	
	// GetParty
	public Party<Entity> GetParty() {
		return party;
	}
	
	// GetAccountId
	public virtual string GetAccountId() {
		return null;
	}
#endregion
	
#region Log
	// Debug log with player name as prefix
	protected void Log(object message) {
		LogManager.General.Log(logPrefix + message);
	}
	
	// Debug log warning with player name as prefix
	protected void LogWarning(object message) {
		LogManager.General.LogWarning(logPrefix + message);
	}
	
	// Debug log error with player name as prefix
	protected void LogError(object message) {
		LogManager.General.LogError(logPrefix + message);
	}
	
	// Debug log with player name as prefix
	protected void LogSpam(object message) {
		LogManager.Spam.Log(logPrefix + message);
	}
#endregion
	
#region Properties
	// --------------------------------------------------------------------------------
	// Properties
	// --------------------------------------------------------------------------------
	
	// Player name
	new public string name {
		get {
			return _entityName;
		}
		
		set {
			_entityName = value;

			// Update game object name
			UpdateName();
			
			// Update name label
			UpdateNameLabelText();
		}
	}

	// Level
	public virtual double level {
		get {
			return 0d;
		}
	}

	// Position
	public Vector3 position {
		get {
			return myTransform.position;
		}
	}

	// Is ready
	public bool isReady {
		get;
		set;
	}

	// Guild name
	protected virtual string guildName {
		get {
			return _guildName;
		}
		
		set {
			_guildName = value;
		}
	}
	
	// Guild tag
	protected string guildTag {
		get {
			return _guildTag;
		}
		
		set {
			_guildTag = value;
			
			// Update name label for guild info
			UpdateNameLabelText();
		}
	}

	// Server position
	public Vector3 serverPosition {
		get;
		set;
	}

	// Client position
	public Vector3 clientPosition {get; set;}
	
	// Char graphics model
	public Transform charGraphicsModel {
		get {
			return _charGraphicsModel;
		}
		
		set {
			_charGraphicsModel = value;
			characterDefinition = _charGraphicsModel.GetComponent<CharacterDefinition>();
			charGraphicsBody = characterDefinition.body;
			leftHand = characterDefinition.leftHand;
			rightHand = characterDefinition.rightHand;
			hips = characterDefinition.hips;
			weaponBone = characterDefinition.weaponBone;

			// Animator
			if(Config.instance.disableAnimationsOnServer && uLink.Network.isServer) {
				// Disable animations on server
				var tmpAnimator = charGraphicsModel.GetComponent<Animator>();
				if(tmpAnimator != null)
					tmpAnimator.enabled = false;
			} else {
				// Get animator for the client
				animator = charGraphicsModel.GetComponent<Animator>();
				
				// Set animator layer weight
				if(animator.enabled && animator.layerCount > 3) {
					animator.SetLayerWeight(1, 1);
					animator.SetLayerWeight(2, 1);
					animator.SetLayerWeight(3, 1);
				}
			}
		}
	}
	
	// Party
	public GameServerParty party {
		get;
		set;
	}
	
	// Score
	public int score {
		get {
			return stats.total.kills * 100 + (int)stats.total.damage / 100 + (int)stats.total.cc;
		}
	}
	
	// Spawn protection
	public bool hasSpawnProtection {
		get {
			return uLink.Network.time - lastRespawn <= Config.instance.spawnProtectionDuration;
		}
	}
	
	// Collision layer
	public int layer {
		get {
			return gameObject.layer;
		}
		
		set {
			MoveToLayer(myTransform, value);
		}
	}

	// Skill layer
	public int skillLayer {
		get;
		set;
	}
	
	// Layer mask for enemies
	public int enemiesLayerMask {
		get {
			// Enemies are in all layers except for own layer and "ignore raycast" layer
			return ~(1 << gameObject.layer | Physics.kIgnoreRaycastLayer);
		}
	}
	
	// Height
	public float height {
		get {
			return characterController.height * heightMultiplier;
		}
	}
	
	// Center point
	public Vector3 center {
		get {
			//LogManager.General.Log("Center offset: " + centerOffset);
			return myTransform.position + centerOffset;
		}
	}
	
	// Center of hands
	public Vector3 handsCenter {
		get {
			return (leftHand.position + rightHand.position) / 2;
		}
	}

	// Skeleton
	public Transform rightHand {get; set;}
	public Transform leftHand {get; set;}
	public Transform hips {get; set;}
	public Transform weaponBone {get; set;}

	// Components
	public Controller controller {get; set;}
	public CrossHair crossHair {get; protected set;}
	public CharacterDefinition characterDefinition {get; protected set;}
	public EntityLabel nameLabel {get; protected set;}
	public EntityGUI entityGUI {get; protected set;}
	public CharacterController characterController {get; protected set;}
	public ComboCounter comboCounter {get; protected set;}
	public Animator animator {get; protected set;}
#endregion
	
#region RPCs
	// --------------------------------------------------------------------------------
	// RPCs
	// --------------------------------------------------------------------------------

	[RPC]
	protected void RegisterKill(ushort killerId, ushort victimId, short skillId) {
		Log("RegisterKill: " + killerId + ", " + victimId + ", " + skillId);

		// Find entities
		Entity victim;
		Entity killer;
		
		if(!Entity.idToEntity.TryGetValue(victimId, out victim)) {
			LogError("Could not find entity with ID: " + victimId);
			return;
		}

		if(!Entity.idToEntity.TryGetValue(killerId, out killer)) {
			LogError("Could not find entity with ID: " + killerId);
			return;
		}

		// Update stats
		killer.stats.total.kills += 1;
		victim.stats.total.deaths += 1;

		// On kill
		if(killer.onKill != null)
			killer.onKill(killer, victim, skillId);

		// On death
		if(victim.onDeath != null)
			victim.onDeath();
	}
#endregion
}
