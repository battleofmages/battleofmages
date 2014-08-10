using UnityEngine;

// Global
public class Config : SingletonMonoBehaviour<Config> {
	// Settings
	public const float rotationInterpolationSpeed = 25f;
	public const float maxSquaredDistanceOwnColliderAllowedInLoS = 6f * 6f;

	// Root objects
	public static Transform particlesRoot;

	[Header("Prefabs")]
	public GameObject femalePrefab;
	public GameObject malePrefab;
	public GameObject damageNumber;
	public GameObject dangerDetector;
	public GameObject blockSphere;
	public GameObject lootTrail;
	public GameObject levelUpEffect;

	public Font font;

	[Header("Misc")]

	[Range(1, 15)]
	public float playerMoveSpeed;

	[Range(100, 10000)]
	public int playerHP;

	[Range(0, 100)]
	public float entityEnergy;

	[Range(1, 10)]
	public float blockMinimumEnergyForUsage = 5.0f;

	[Range(1, 20)]
	public float hoverEnergyCost = 9.0f;

	[Range(0, 1)]
	public float hoverSpeedBonus = 0.7f;

	[Range(0, 1)]
	public float blockSlowDown = 0.5f;

	[Range(1, 10)]
	public float energyRegen = 4.0f;

	[Range(1, 100)]
	public float blockEnergyDrain = 40.0f;

	[Range(100, 200)]
	public float entityVisibilityDistance;

	public AnimationCurve playerLabelAlphaWithDistance;
	public float raycastMaxDistance;

	public float playerRespawnTime;
	public double spawnProtectionDuration;
	public float deathColliderDisableTime;
	public float skillInstanceDestructionTime;
	public float matchAcceptTime;

	[Header("Physics")]
	public int openWorldPvPLayer;
	public int skillLayersStart;
	public int skillLayersCount;

	[Header("Enemies")]

	[Range(0, 1)]
	public float enemyReactionTime;
	
	[Range(100, 10000)]
	public int hpAggroThreshold;
	
	[Range(0, 1)]
	public float patrolSpeedModifier;

	[Header("GUI")]
	public int ownDmgOffset;
	public int healthBarWidth;
	public int ownHealthBarWidth;
	public Color expColor;

	[Header("Experience")]

	[Range(1, 10)]
	public int playerLevelToExperience;

	[Range(1, 10)]
	public int enemyLevelToExperience;

	[Header("Lag compensation")]

	[Range(0, 1)]
	public float serverPositionPredictionFactor;

	public float maxPositionPredictionTime;
	public float maxProxyDistanceUntilSnapSqr;
	public float proxyInterpolationSpeed;
	public float ownerInterpolationSpeed;
	
	[Header("Server")]
	public bool disableAnimationsOnServer;
	public float enemyRespawnTime;

	[Range(0.5f, 1)]
	public float matchStatsSendDelay;

	[Range(1, 3)]
	public float pingSendDelay;

	[Range(1, 3)]
	public float savePositionDelay;

	[Range(1, 3)]
	public float saveExperienceDelay;

	// Awake
	protected override void Awake() {
		// Root objects
		if(GameObject.Find("Root") != null) {
			Config.particlesRoot = GameObject.Find("Root/Particles").transform;
			Entity.skillInstancesRoot = GameObject.Find("Root/SkillInstances").transform;
			Player.playerRoot = GameObject.Find("Root/Players").transform;
			ObjectLabel.labelRoot = GameObject.Find("Root/Labels").transform;
			Enemy.enemiesRoot = GameObject.Find("Root/Enemies").transform;
		}
		
		base.Awake();
	}
}
