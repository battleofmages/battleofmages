using UnityEngine;

public abstract partial class Entity : uLink.MonoBehaviour, PartyMember<Entity> {
	public float baseMoveSpeed;

	// Cached
#region Health
	private int _maxHealth;
	private int _health = 0;

	protected float curHealthRatio = 1.0f;
	
	protected int _healthBarWidth = 200;
	protected int healthBarHeight = 24;
	
	[HideInInspector]
	public float curHealthBarWidth;
#endregion

#region Energy
	[HideInInspector]
	public float energy;
#endregion

#region Modifiers
	[HideInInspector]
	public CharacterStats charStats;

	[HideInInspector]
	public float moveSpeedModifier = 1.0f;
	
	[HideInInspector]
	public float attackDmgMultiplier = 1.0f;
	
	[HideInInspector]
	public float defenseDmgMultiplier = 1.0f;
	
	[HideInInspector]
	public float attackSpeedMultiplier = 1.0f;
#endregion

#region Methods
	// Constructor
	void InitTraits() {
		charStats = null;
		
		maxHealth = Config.instance.playerHP;
		health = maxHealth;
		
		maxEnergy = Config.instance.entityEnergy;
		energy = maxEnergy;
	}

	// Apply energy drain
	public bool ApplyEnergyDrain() {
		if(!isAlive) {
			energy = 0.0f;
			return false;
		}
		
		// Hover
		if(hovering)
			energy -= Config.instance.hoverEnergyCost * Time.deltaTime;
		
		// Block
		if(blocking) {
			energy -= Config.instance.blockEnergyDrain * Time.deltaTime;
		} else if(!hovering) {
			// Energy regeneration
			if(energy < maxEnergy)
				energy += Config.instance.energyRegen * Time.deltaTime;
			
			// Maximum energy cap
			if(energy > maxEnergy)
				energy = maxEnergy;
		}
		
		// Should never go below 0
		if(energy < 0)
			energy = 0;
		
		return true;
	}

	// Updates energy value
	protected void UpdateEnergyOnServer() {
		if(ApplyEnergyDrain() && energy < 0.01f) {
			// This will cancel blocking and hovering on each client
			networkView.RPC("SetEnergy", uLink.RPCMode.Others, 0f);
			SetEnergy(0f);
		}
	}
#endregion

#region Properties
	// Max HP
	public int maxHealth {
		get {
			return _maxHealth;
		}
		
		set {
			_maxHealth = value;
			curHealthRatio = _health / (float)_maxHealth;
			curHealthBarWidth = curHealthRatio * _healthBarWidth;
		}
	}
	
	// Current HP
	public int health {
		get {
			return _health;
		}
		
		set {
			_health = value;
			
			if(uLink.Network.isServer) {
				if(_health > _maxHealth)
					_health = _maxHealth;
				
				if(_health < 0)
					_health = 0;
			} else {
				curHealthRatio = _health / (float)_maxHealth;
				curHealthBarWidth = curHealthRatio * _healthBarWidth;
			}
		}
	}

	// Max energy
	public float maxEnergy {
		get;
		set;
	}

	// Energy ratio
	public float energyRatio {
		get {
			return energy / maxEnergy;
		}
	}
	
	// Is alive
	public bool isAlive {
		get {
			return _health > 0;
		}
	}

	// Health bar width
	public int healthBarWidth {
		get {
			return _healthBarWidth;
		}
		
		set {
			_healthBarWidth = value;
			curHealthBarWidth = curHealthRatio * _healthBarWidth;
		}
	}
	
	// Move speed
	public float moveSpeed {
		get {
			if(moveSpeedModifier < 0)
				return 0.0f;
			
			if(blocking)
				return baseMoveSpeed * moveSpeedModifier * Config.instance.blockSlowDown;
			
			return baseMoveSpeed * moveSpeedModifier;
		}
	}
#endregion

#region RPCs
	[RPC]
	protected void SetHP(ushort newHP) {
		health = newHP;
	}
	
	[RPC]
	protected void SetEnergy(float newEnergy) {
		energy = newEnergy;
		
		if(energy == 0f) {
			if(blocking)
				EndBlock();
			
			if(hovering)
				EndHover();
		}
	}
#endregion
}
