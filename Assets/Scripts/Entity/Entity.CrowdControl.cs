using UnityEngine;
using System;
using System.Collections;

public abstract partial class Entity : uLink.MonoBehaviour, PartyMember<Entity> {
	[NonSerialized]
	public bool hasControlOverMovement = true;

	[NonSerialized]
	public bool blocking;
	
	[NonSerialized]
	public bool jumping;
	
	[NonSerialized]
	public bool hovering;
	
	[NonSerialized]
	public int stunned;
	
	[NonSerialized]
	public int immobilized;
	
	[NonSerialized]
	public int stagger;
	
	[NonSerialized]
	public int slept;

	[NonSerialized]
	public int immuneToPull;

#region Methods
	// StartStun
	public void StartStun() {
		stunned += 1;
		InterruptCast();
		
		if(animator != null)
			animator.SetInteger("Stunned", slept + stunned);
	}
	
	// EndStun
	public void EndStun() {
		stunned -= 1;
		
		if(animator != null)
			animator.SetInteger("Stunned", slept + stunned);
	}
	
	// StartStagger
	public void StartStagger() {
		stagger += 1;
		InterruptCast();
		
		if(animator != null)
			animator.SetInteger("Stagger", stagger);
	}
	
	// EndStagger
	public void EndStagger() {
		stagger -= 1;
		
		if(animator != null)
			animator.SetInteger("Stagger", stagger);
	}
	
	// StartSleep
	public void StartSleep() {
		slept += 1;
		InterruptCast();
		
		if(uLink.Network.isClient) {
			if(animator != null)
				animator.SetInteger("Stunned", slept + stunned);
			
			// Only spawn "zZz" text for first one
			if(slept == 1)
				Invoke("SpawnSleepText", 0.333f);
		}
	}
	
	// SpawnSleepText
	public void SpawnSleepText() {
		if(slept > 0) {
			Entity.SpawnText(this, "z", new Color(0.75f, 0.75f, 0.75f, 1.0f), UnityEngine.Random.Range(-5, 5), -20);
			Invoke("SpawnSleepText", 0.333f);
		}
	}
	
	// EndSleep
	public void EndSleep() {
		if(slept > 0)
			slept -= 1;
		
		if(animator != null)
			animator.SetInteger("Stunned", slept + stunned);
	}
#endregion

#region Properties
	// Can cast
	public bool canCast {
		get {
			return isAlive && !blocking && stunned == 0 && slept == 0 && stagger == 0;
		}
	}
	
	// Can block
	public bool canBlock {
		get {
			return blockingEnabled && isAlive && !blocking && stunned == 0 && slept == 0 && stagger == 0 && energy > Config.instance.blockMinimumEnergyForUsage;
		}
	}
	
	// Can hover
	public bool canHover {
		get {
			return isAlive && stunned == 0 && immobilized == 0 && slept == 0 && stagger == 0;
		}
	}
	
	// Can't move
	public bool cantMove {
		get {
			return (
				stagger > 0 ||
				stunned > 0 ||
				immobilized > 0 ||
				slept > 0 ||
				!hasControlOverMovement ||
				moveSpeed <= 0 ||
				GameManager.gameEnded ||
				!isAlive ||
				(isCasting && !currentSkill.currentStage.canMoveWhileCasting) ||
				(isAttacking && !currentSkill.currentStage.canMoveWhileAttacking)
			);
		}
	}

	// Can be pulled
	public bool canBePulled {
		get {
			return isAlive && !blocking && !hasSpawnProtection && immuneToPull == 0;
		}
	}
#endregion

#region RPCs
	[RPC]
	protected IEnumerator Stagger(float duration) {
		StartStagger();
		yield return new WaitForSeconds(duration);
		EndStagger();
	}
#endregion
}
