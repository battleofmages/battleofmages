using UnityEngine;

// Global
public class Config : SingletonMonoBehaviour<Config> {
	// Settings
	public const float rotationInterpolationSpeed = 25f;
	public const float maxSquaredDistanceOwnColliderAllowedInLoS = 6f * 6f;

	// Root objects
	public static Transform particlesRoot;
	
	// Prefabs
	public GameObject femalePrefab;
	public GameObject malePrefab;
	public GameObject damageNumber;
	public GameObject dangerDetector;
	public GameObject blockSphere;
	public GameObject lootTrail;

	public GUISkin guiSkin;
	
	public float playerMoveSpeed;
	public int playerHP;
	public float entityEnergy;
	public float blockMinimumEnergyForUsage = 5.0f;
	public float hoverEnergyCost = 9.0f;
	public float hoverSpeedBonus = 0.7f;
	public float blockSlowDown = 0.5f;
	public float energyRegen = 4.0f;
	public float blockEnergyDrain = 40.0f;

	// Enemies
	public float enemyReactionTime;
	public int hpAggroThreshold;
	public float patrolSpeedModifier;
	
	public float entityVisibilityDistance;
	public AnimationCurve playerLabelAlphaWithDistance;
	public float raycastMaxDistance;

	public float playerRespawnTime;
	public double spawnProtectionDuration;
	public float deathColliderDisableTime;
	public int ownDmgOffset;

	public float serverPositionPredictionFactor;
	public float proxyInterpolationSpeed;
	public float maxProxyDistanceUntilSnapSqr;
	public float ownerInterpolationSpeed;
	public float maxPositionPredictionTime;

	public int healthBarWidth;
	public int ownHealthBarWidth;

	public float skillInstanceDestructionTime;

	public float matchAcceptTime;

	// Server
	public bool disableAnimationsOnServer;
	public float enemyRespawnTime;
	public float matchStatsSendDelay;
	public float pingSendDelay;
	public float savePositionDelay;
	
	// Awake
	protected override void Awake() {
		// Root objects
		if(GameObject.Find("Root") != null) {
			Config.particlesRoot = GameObject.Find("Root/Particles").transform;
			Entity.skillInstancesRoot = GameObject.Find("Root/SkillInstances").transform;
			Player.playerRoot = GameObject.Find("Root/Players").transform;
			EntityLabel.labelRoot = GameObject.Find("Root/Labels").transform;
			Enemy.enemiesRoot = GameObject.Find("Root/Enemies").transform;
		}
		
		base.Awake();
	}
}
