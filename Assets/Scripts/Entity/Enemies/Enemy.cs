using UnityEngine;
using System.Collections.Generic;

public class Enemy : Entity {
	public static List<Enemy> allEnemies = new List<Enemy>();
	public static Transform enemiesRoot;

	public GameObject prefab;
	
	// Awake
	protected override void Awake() {
		base.Awake();

		// Parent
		myTransform.parent = Enemy.enemiesRoot;

		charStats = new CharacterStats();
		skillBuild = SkillBuild.GetStarterBuild();
		
		runes = new List<Rune>();
		weapons = Magic.instance.GetWeapons(runes, skillBuild);
		moveSpeedModifier = 1.0f;
		
		charGraphicsModel = InstantiateChild(prefab, charGraphics);

		// Set name
		name = gameObject.name.ToCleanGameObjectName();
		
		// Visibility
		isVisible = false;

		// Layer
		layer = gameObject.layer;

		// We use 1 layer below the game object layer for the enemy skills
		skillLayer = layer - 1;

		// Death
		onDeath += () => {
			if(animator != null)
				animator.SetBool(deadHash, true);
			
			Invoke("DisableCollider", Config.instance.deathColliderDisableTime);
		};

		// Destroy
		onDestroy += () => {
			// Remove from global lists
			Enemy.allEnemies.Remove(this);
			Entity.idToEntity.Remove(id);
			
			// In case we stored some old pointers, react as if he is dead.
			// Particularly useful for the Enemy threat based target finding.
			health = 0;
		};
		
		// Add to global list
		Enemy.allEnemies.Add(this);
	}

	// OnTargetReceived
	protected override void OnTargetReceived() {
		if(!hasTarget) {
			//DLog("Doesn't have a target anymore, interrupting cast: " + currentSkill);
			InterruptCast();
		}
	}
	
	// DisableCollider
	void DisableCollider() {
		collider.enabled = false;
	}

	// GetHitPoint
	protected override Vector3 GetHitPoint() {
		return castStart.target.center;
	}
}