using UnityEngine;
using System.Collections;

public class Control : MonoBehaviour {
	public Health health;
	public Energy energy;

	public bool blockingEnabled;

	[System.NonSerialized]
	public bool hasControlOverMovement = true;

	[System.NonSerialized]
	public bool blocking;

	[System.NonSerialized]
	public bool jumping;

	[System.NonSerialized]
	public bool hovering;

	[System.NonSerialized]
	public int stunned;

	[System.NonSerialized]
	public int immobilized;

	[System.NonSerialized]
	public int stagger;

	[System.NonSerialized]
	public int slept;

	[System.NonSerialized]
	public int immuneToPull;

	[System.NonSerialized]
	public bool hasSpawnProtection;

	#region Properties

	// Can cast
	public bool canCast {
		get {
			return health.available && !blocking && stunned == 0 && slept == 0 && stagger == 0;
		}
	}
	
	// Can block
	public bool canBlock {
		get {
			return blockingEnabled && health.available && !blocking && stunned == 0 && slept == 0 && stagger == 0 && energy.current > Config.instance.blockMinimumEnergyForUsage;
		}
	}
	
	// Can hover
	public bool canHover {
		get {
			return health.available && stunned == 0 && immobilized == 0 && slept == 0 && stagger == 0;
		}
	}
	
	/*// Can't move
	public bool cantMove {
		get {
			return (
				stagger > 0 ||
				stunned > 0 ||
				immobilized > 0 ||
				slept > 0 ||
				!hasControlOverMovement ||
				!health.available ||
				(isCasting && !currentSkill.currentStage.canMoveWhileCasting) ||
				(isAttacking && !currentSkill.currentStage.canMoveWhileAttacking)
			);
		}
	}*/
	
	// Can be pulled
	public bool canBePulled {
		get {
			return health.available && !blocking && !hasSpawnProtection && immuneToPull == 0;
		}
	}

	// Is casting
	public bool isCasting {
		get;
		protected set;
	}
	
	// Is attacking
	public bool isAttacking {
		get;
		protected set;
	}

	#endregion
}
