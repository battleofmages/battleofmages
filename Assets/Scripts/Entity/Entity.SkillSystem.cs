using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract partial class Entity : uLink.MonoBehaviour, PartyMember<Entity> {
	public static Transform skillInstancesRoot;

	// Animation layer
	public static class AnimationLayer {
		public const int Base = 0;
		public const int Skill = 1;
		public const int Block = 2;
		public const int NoControl = 3;
	}

	// Hashes
	public static int noCastState = Animator.StringToHash("SkillLayer.NoCast");
	public static int interruptedHash = Animator.StringToHash("Interrupted");
	public static int skillEndedHash = Animator.StringToHash("SkillEnded");
	public static int deadHash = Animator.StringToHash("Dead");

	// Weapons
	[NonSerialized]
	public List<Weapon> weapons;
	
	[NonSerialized]
	public Skill selectedSkill;

	[NonSerialized]
	private SkillBuild _skillBuild;
	
	[NonSerialized]
	public Dictionary<int, int> skillDamageReceived;
	
	[NonSerialized]
	public Skill lastHitBySkill;

	[NonSerialized]
	public float magicCircleYRotation = 0f;
	
	protected bool advancing;

	protected ushort currentSpellId;
	protected ushort lastEndCastSpellId;

	[System.NonSerialized]
	public double currentCastStart = -1d;

	// Cast start
	protected struct CastStart {
		public Entity target;
	}
	protected CastStart castStart;

	// Bypass cooldown
	private bool skillTestMode = false;

	// Mecanim state
	AnimatorStateInfo skillAnimationState;

#region Methods
	// Constructor
	void InitSkillSystem() {
		skillEffects = new List<SkillEffect>();
		skillEffectsTimeApplied = new List<double>();
		skillDamageReceived = new Dictionary<int, int>();
		hitsByPlayer = new Dictionary<Entity, int>();

		// Don't let them have the same value
		currentSpellId = 0;
		lastEndCastSpellId = ushort.MaxValue;
	}

	// Activates a skill
	public SkillInstance UseSkill(Skill skill, Vector3 hitPoint, int stageIndex = -1, float lastUsage = 0f) {
		// Transform
		Vector3 skillPosition = Cache.vector3Zero;
		Quaternion skillRotation = Cache.quaternionIdentity;
		
		if(stageIndex == -1)
			stageIndex = skill.currentStageIndex;
		
		var skillStage = skill.stages[stageIndex];
		
		if(skillStage == null)
			return null;
		
		// Position
		switch(skillStage.posType) {
			case Skill.PositionType.AtHitPoint:
			case Skill.PositionType.AtGround:
				skillPosition = hitPoint;
				break;
				
			case Skill.PositionType.AtRightHand:
				skillPosition = rightHand.position;
				break;
				
			case Skill.PositionType.AtCasterFeet:
				skillPosition = myTransform.position;
				break;
		}
		
		//skillPosition += skillStage.posOffset;
		skillPosition.y += skillStage.posOffset.y;
		
		// Rotation
		switch(skillStage.rotType) {
			case Skill.RotationType.ToCasterOnY:
				skillRotation = Quaternion.LookRotation(myTransform.position - skillPosition);
				
				// Correct rotation, only Y applied
				Vector3 angles = skillRotation.eulerAngles;
				angles.x = 0;
				angles.z = 0;
				skillRotation.eulerAngles = angles;
				break;
				
			case Skill.RotationType.ToHitPoint:
				var diff = hitPoint - skillPosition;
				if(diff != Cache.vector3Zero)
					skillRotation = Quaternion.LookRotation(diff);
				else
					LogManager.General.LogError("Skill spawned at hitpoint and has a rotation towards hitpoint: That doesn't work");
				break;
				
			case Skill.RotationType.Up:
				// TODO: Can be cached
				skillRotation = Cache.upRotation;
				break;
		}
		
		// Clone it
		GameObject prefab = skill.prefabs[stageIndex];
		SkillInstance inst = null;
		
		if(prefab != null) {
			inst = SpawnSkillInstance(
				prefab,
				skill,
				skillStage,
				skillPosition,
				skillRotation,
				hitPoint
			);
		}
		
		// Absorb energy costs
		energy -= skillStage.energyCostAbs;
		
		// Put all skill stages on cooldown
		foreach(var stage in skill.stages) {
			if(stage == null)
				continue;
			
			if(lastUsage == 0f)
				stage.lastUse = uLink.Network.time;
			else
				stage.lastUse = lastUsage;
		}
		
		return inst;
	}
	
	// Spawn skill instance
	protected SkillInstance SpawnSkillInstance(
		UnityEngine.Object prefab,
		Skill skill,
		Skill.Stage skillStage,
		Vector3 skillPosition,
		Quaternion skillRotation,
		Vector3 hitPoint
	) {
		GameObject clone = (GameObject)UnityEngine.Object.Instantiate(prefab, skillPosition, skillRotation);
		clone.transform.parent = skillInstancesRoot;
		
		// Disable some stuff on server
		if(uLink.Network.isServer) {
			// Audio on server
			if(clone.audio != null) {
				Destroy(clone.audio);
			}
			
			/*// Particles on server
			if(clone.particleEmitter != null)
				clone.particleEmitter.enabled = false;
			
			var pSystem = clone.particleSystem;
			if(pSystem != null) {
				pSystem.enableEmission = false;
				if(!pSystem.isStopped)
					pSystem.Stop();
			}*/
		}
		
		// Set skill instance info
		lastSkillInstance = clone.GetComponent<SkillInstance>();
		lastSkillInstance.caster = this;
		lastSkillInstance.skill = skill;
		lastSkillInstance.skillStage = skillStage;
		lastSkillInstance.hitPoint = hitPoint;
		
		//LogManager.General.Log("Skill stage: " + skillStage + " | Index: " + stageIndex + ", " + skillStage.powerMultiplier);
		
		// Ignore collision with caster's team
		// HINT: Change this to Entity.MoveToLayer() if you have collider children
		clone.layer = skillLayer;
		
		// Destroy after a few seconds
		if(!skill.canHold)
			Destroy(clone, Config.instance.skillInstanceDestructionTime);
		
		return lastSkillInstance;
	}
	
	// Checks if the user wants to cast a new skill
	protected void UpdateStartCast() {
		if(currentSkill == null && controller.canStartCast && canCast)
			TryStartCast(controller.GetNextSkill());

		// Skill test mode
		if(Debugger.instance != null)
			skillTestMode = Debugger.instance.skillTestMode;

		// Skill advancement
		if(currentSkill != null) {
			if(controller.holdsSkill) {
				if(currentSkill.canHold && lastSkillInstance)
					lastSkillInstance.hitPoint = GetHitPoint();
			} else {
				advancing = false;
				
				// End skill we are holding
				if(StopHoldingSkill())
					networkView.RPC("ClientStopHolding", uLink.RPCMode.Server);
			}
		}
	}

	// TryStartCast
	private void TryStartCast(byte slotId) {
		if(slotId == byte.MaxValue)
			return;

		// We set this here as well to prevent spamming the server with skill cast requests
		selectedSkill = skills[slotId];
		
		if(selectedSkill.currentStage == null)
			return;

		// On cooldown?
		if(selectedSkill.currentStage.isOnCooldown && !skillTestMode) {
			controller.OnSkillIsOnCooldown();
			return;
		}

		// Not enough energy?
		if (this.energy < selectedSkill.currentStage.energyCostAbs) {
			controller.OnNotEnoughEnergyForSkillCast();
			return;
		}

		// Seems we can cast it, set current skill
		currentSkill = selectedSkill;
		
		// Save target at the start of casting
		castStart.target = this.target;
		
		// Increase spell ID
		currentSpellId += 1;

		// Instant cast
		if(currentSkill.currentStage.isInstantCast) {
			var hitPoint = GetHitPoint();
			
			if(!skillTestMode)
				networkView.RPC("ClientInstantCast", uLink.RPCMode.Server, slotId, hitPoint);
			
			InstantCast(slotId, hitPoint);
			return;
		}

		// Normal cast
		if(!skillTestMode)
			networkView.RPC("ClientStartCast", uLink.RPCMode.Server, slotId);
		
		StartCast(slotId);

		// Start with the assumption that we'll advance
		advancing = true;
	}

	// Ends cast or advances skill
	// This function is automatically called after the cast duration of the skill
	protected IEnumerator UpdateEndCast(float castDuration, ushort spellId) {
		yield return new WaitForSeconds(castDuration);
		
		// Old end cast, we already have another coroutine running for this one.
		// This happens if you start a cast, block instantly and start a cast again.
		if(spellId != currentSpellId) {
			LogSpam("UpdateEndCast canceled: Spell ID doesn't match (" + spellId + " vs " + currentSpellId + ")");
			yield break;
		}

		if(currentSkill == null) {
			LogSpam("UpdateEndCast canceled: currentSkill is null");
			yield break;
		}
		
		// Control?
		if(blocking || stunned > 0 || slept > 0 || stagger > 0) {
			networkView.RPC("ClientCouldntCast", uLink.RPCMode.Server);
			LogSpam("UpdateEndCast canceled: No control");
			yield break;
		}
		
		// End cast
		if(advancing && (currentSkill.canAdvance || (skillTestMode && currentSkill.hasNextStage))) {
			// TODO: ...
			if(!skillTestMode)
				networkView.RPC("ClientAdvanceCast", uLink.RPCMode.Server);
			
			AdvanceCast();
		} else {
			var hitPoint = GetHitPoint();
			
			if(!skillTestMode)
				networkView.RPC("ClientEndCast", uLink.RPCMode.Server, hitPoint);
			
			EndCast(hitPoint);
		}
	}
	
	// Starts the casting animation
	protected void RestartCast() {
		currentCastStart = uLink.Network.time;
		
		// Animation
		SetCastAnimationState(1, 0);
		
		// Cast animation
		if(currentSkill.currentStage.castEffectPrefab != null) {
			StartCastEffect(currentSkill.currentStage.castEffectPrefab);
		} else {
			StopCastEffect();
		}
		
		// Cast voice
		var castVoices = currentSkill.currentStage.castVoices;

		if(castVoices.Length > 0 && audio != null) {
			var voiceClip = castVoices[UnityEngine.Random.Range(0, castVoices.Length)];

			if(voiceClip != null)
				audio.PlayOneShot(voiceClip);
			else
				LogWarning("Cast voice clip is null: " + currentSkill);
		}
		
		// AdvanceCast or EndCast skill
		OnCastRestart();
	}
	
	// Animation state
	protected void SetCastAnimationState(int castState = -1, int fireState = -1) {
		if(currentSkill == null || animator == null)
			return;

		switch(currentSkill.currentStage.animType) {
			case Skill.CastAnimation.Projectile:
				if(castState != -1)
					animator.SetBool("CastProjectile", castState != 0);
				if(fireState != -1)
					animator.SetBool("FireProjectile", fireState != 0);
				break;
				
			case Skill.CastAnimation.AoE:
				if(castState != -1)
					animator.SetBool("CastAoE", castState != 0);
				if(fireState != -1)
					animator.SetBool("FireAoE", fireState != 0);
				break;
				
			case Skill.CastAnimation.Sword:
				if(castState != -1)
					animator.SetBool("CastSword", castState != 0);
				if(fireState != -1)
					animator.SetBool("FireSword", fireState != 0);
				break;
				
			case Skill.CastAnimation.HandRDown:
				if(castState != -1)
					animator.SetBool("CastHandRDown", castState != 0);
				if(fireState != -1)
					animator.SetBool("FireHandRDown", fireState != 0);
				break;
				
			case Skill.CastAnimation.HandsLDownRUp:
				if(castState != -1)
					animator.SetBool("CastHandsLDownRUp", castState != 0);
				if(fireState != -1)
					animator.SetBool("FireHandsLDownRUp", fireState != 0);
				break;
				
				// Instant casts:
				
			case Skill.CastAnimation.SideSlashLToR:
				if(fireState != -1)
					animator.SetBool("FireSideSlashLToR", fireState != 0);
				break;
				
			case Skill.CastAnimation.DiagonalSlashTopRToBottomL:
				if(fireState != -1)
					animator.SetBool("FireDiagonalSlashTopRToBottomL", fireState != 0);
				break;
				
			case Skill.CastAnimation.Thrust:
				if(fireState != -1)
					animator.SetBool("FireThrust", fireState != 0);
				break;
		}
	}

	// Updates cast animations
	protected void UpdateSkillAnimations() {
		if(currentSkill == null)
			return;

		// Current state
		skillAnimationState = animator.GetCurrentAnimatorStateInfo(AnimationLayer.Skill);
		
		// Client sets current skill to null when he is in noCastState
		if(currentSpellId == lastEndCastSpellId && animator.GetBool(skillEndedHash) && skillAnimationState.nameHash == noCastState) {
			SetCastAnimationState(0, 0);
			animator.SetBool(skillEndedHash, false);
			currentSkill = null;
		}
	}

	// Ends the current skill so that the player can cast the next one
	public void EndSkill() {
		// Commented out because I think this might lead to animations getting stuck
		//if(currentSkill == null)
		//	return;
		
		if(animator)
			animator.SetBool(skillEndedHash, true);
		
		if(uLink.Network.isClient)
			StopCastEffect();
		
		// Server sets it instantly to null
		// Client has to wait for the animation to end
		// Proxy unclear yet (?)
		if(uLink.Network.isServer) //|| networkViewIsProxy) {
			currentSkill = null;
	}
#endregion

#region Properties
	// Attunements
	public List<Attunement> attunements {
		get { return currentWeapon.attunements; }
	}
	
	// Skills
	public List<Skill> skills {
		get { return currentAttunement.skills; }
	}

	// Current weapon
	public Weapon currentWeapon {get; set;}

	// Current attunement
	public Attunement currentAttunement {get; set;}

	// Current skill
	private Skill _currentSkill;
	public Skill currentSkill {
		get {
			return _currentSkill;
		}

		set {
			if(_currentSkill == value)
				return;

			_currentSkill = value;

			if(_currentSkill == null && animator != null) {
				if(skillAnimationState.nameHash != noCastState && !animator.GetBool(interruptedHash)) {
					InterruptCast();
				}
			}
		}
	}

	// Last skill instance
	protected SkillInstance lastSkillInstance {get; set;}
	
	// Is casting
	public bool isCasting {
		get {
			return currentSkill != null && currentCastStart != -1d;
		}
	}
	
	// Is attacking
	public bool isAttacking {
		get {
			return currentSkill != null && currentCastStart == -1d;
		}
	}
	
	// Is holding skill
	public bool isHoldingSkill {
		get {
			return currentSkill != null && currentSkill.canHold && lastSkillInstance;
		}
	}

	// Skill build
	public SkillBuild skillBuild {
		get {
			return _skillBuild;
		}
		
		set {
			_skillBuild = value;
			
			if(_skillBuild == null) {
				runes = null;
				weapons = null;
				currentWeapon = null;
				currentAttunement = null;
				return;
			}
			
			runes = new List<Rune>();
			weapons = Magic.instance.GetWeapons(runes, skillBuild);
			
			// Current weapon
			currentWeapon = weapons[0];
			
			// Current attunement
			currentAttunement = attunements[0];
		}
	}
#endregion

#region Virtual
	// InterruptCast
	public virtual void InterruptCast() {
		if(currentSkill != null) {
			LogSpam("Cast has been interrupted: " + currentSkill.skillName);
			
			// Skill we were holding
			if(StopHoldingSkill())
				networkView.RPC("ClientStopHolding", uLink.RPCMode.Server);
		}
		
		// Destroy cast particles
		StopCastEffect();
		SetCastAnimationState(0, 0);

		if(animator != null)
			animator.SetBool(interruptedHash, true);
		
		if(hovering)
			EndHover();
		
		currentSkill = null;
	}
	
	// OnCastRestart
	protected virtual void OnCastRestart() {}

	// StartCastEffect
	protected virtual void StartCastEffect(GameObject prefab) {}
	
	// StopCastEffect
	protected virtual void StopCastEffect() {}

	// StartMeleeAttack
	public virtual void StartMeleeAttack() {}
	
	// StopMeleeAttack
	public virtual void StopMeleeAttack() {}
#endregion

#region RPCs
	[RPC]
	protected void StartCast(byte slotId) {
		if(networkViewIsProxy && currentSkill != null)
			InterruptCast();
		
		// We didn't receive the skill build yet?
		if(skillBuild == null)
			return;
		
		// Just in case
		if(slotId < 0 || slotId >= skills.Count) {
			LogManager.General.LogError(string.Format(
				"Slot ID {0} doesn't exist in skill list with {1} skills on {2}",
				slotId,
				skills.Count,
				networkViewIsMine ? "myself" : "proxy"
			));
			return;
		}
		
		currentSkill = skills[slotId];
		
		// Reset interrupt animation state
		if(animator != null) {
			animator.SetBool(interruptedHash, false);
			animator.SetBool(skillEndedHash, false);
		}
		
		RestartCast();
		
		// On proxies, increase spell ID
		if(networkViewIsProxy)
			currentSpellId += 1;
		
		LogSpam("StartCast: " + currentSkill.skillStageName);
	}
	
	[RPC]
	protected void EndCast(Vector3 hitPoint) {
		if(currentSkill == null) {
			LogWarning("Received EndCast but currentSkill is null!");
			return;
		}
		
		// We didn't receive the skill build yet?
		if(skillBuild == null)
			return;
		
		// Save spell ID
		lastEndCastSpellId = currentSpellId;
		
		// Instantiate it locally
		UseSkill(currentSkill, hitPoint);
		
		// Animation
		SetCastAnimationState(0, 1);

		// Stop the cast effect
		StopCastEffect();

		// End the skill after a delay
		if(!currentSkill.canHold)
			Invoke("EndSkill", currentSkill.currentStage.attackAnimDuration * attackSpeedMultiplier);
		
		// Reset to stage 1
		currentSkill.currentStageIndex = 0;
		
		// No cast bar
		currentCastStart = -1;

		// Log
		LogSpam("EndCast: " + currentSkill.skillStageName);
	}
	
	[RPC]
	protected void AdvanceCast() {
		// Is there a next stage?
		if(currentSkill == null || !currentSkill.canAdvance)
			return;
		
		// We didn't receive the skill build yet?
		if(skillBuild == null)
			return;
		
		// Increase spell ID
		currentSpellId += 1;
		
		// Reset animation variables of the previous stage
		SetCastAnimationState(0, 0);
		
		// Increase stage level
		currentSkill.currentStageIndex += 1;
		
		// Cast the new stage
		RestartCast();

		// Log
		LogSpam("AdvanceCast: " + currentSkill.skillName + " to " + currentSkill.skillStageName);
	}
	
	[RPC]
	protected void InstantCast(byte slotId, Vector3 hitPoint) {
		// We didn't receive the skill build yet?
		if(skillBuild == null)
			return;
		
		// Select skill
		currentSkill = skills[slotId];
		
		// Save spell ID
		lastEndCastSpellId = currentSpellId;
		
		// Instantiate it locally
		UseSkill(currentSkill, hitPoint);

		// Reset interrupt animation state
		if(animator != null) {
			animator.SetBool(interruptedHash, false);
			animator.SetBool(skillEndedHash, false);
		}
		
		// Animation
		SetCastAnimationState(0, 1);

		// Stop the cast effect
		StopCastEffect();

		// End the skill after a delay
		Invoke("EndSkill", currentSkill.currentStage.attackAnimDuration * attackSpeedMultiplier);

		// Log
		LogSpam("InstantCast: " + currentSkill.skillName);
	}

	[RPC]
	protected bool StopHoldingSkill() {
		if(!isHoldingSkill)
			return false;
		
		SkillInstance.DestroyButKeepParticles(lastSkillInstance.gameObject);
		lastSkillInstance = null;
		
		EndSkill();
		
		return true;
	}
#endregion

#region Reject RPCs
	[RPC]
	protected void StartCastRejected(CastError error) {
		LogWarning("StartCast rejected!: " + error.ToString());
		InterruptCast();
	}
	
	[RPC]
	protected void AdvanceCastRejected(CastError error) {
		LogWarning("AdvanceCast rejected!: " + error.ToString());
		InterruptCast();
	}
	
	[RPC]
	protected void EndCastRejected(CastError error) {
		switch(error) {
			case CastError.CurrentSkillNull:
				LogWarning("EndCast rejected!: Didn't start a cast (current skill is null)");
				break;
				
			case CastError.NoControl:
				LogWarning("EndCast rejected!: No control over the character (probably crowd controlled)");
				break;
				
			case CastError.CastSpeedHack:
				LogWarning("EndCast rejected!: Cast speed hack");
				break;
				
			default:
				LogWarning("EndCast rejected!: " + error.ToString());
				break;
		}
		
		InterruptCast();
	}
	
	[RPC]
	protected void InstantCastRejected(CastError error) {
		LogWarning("InstantCast rejected!: " + error.ToString());
		currentSkill = null;
	}
	
	[RPC]
	protected void CouldntCast() {
		InterruptCast();
	}
#endregion
}