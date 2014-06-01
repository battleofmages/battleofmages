[System.Serializable]
public class CharacterStats : JsonSerializable<CharacterStats> {
	public static string[] statNames = new string[] {
		"Attack",
		"Defense",
		"Energy",
		"Cooldown Reduction",
		"Attack Speed",
		"Move Speed"
	};
	
	public int attack;
	public int defense;
	public int block;
	public int cooldownReduction;
	public int attackSpeed;
	public int moveSpeed;

	// Empty constructor
	public CharacterStats() {
		attack = 50;
		defense = 50;
		block = 50;
		cooldownReduction = 50;
		attackSpeed = 50;
		moveSpeed = 50;
	}
	
	// Default value - needs to be separate from empty constructor
	public CharacterStats(int defaultValue) {
		attack = defaultValue;
		defense = defaultValue;
		block = defaultValue;
		cooldownReduction = defaultValue;
		attackSpeed = defaultValue;
		moveSpeed = defaultValue;
	}
	
	// Copy constructor
	public CharacterStats(CharacterStats other) {
		attack = other.attack;
		defense = other.defense;
		block = other.block;
		cooldownReduction = other.cooldownReduction;
		attackSpeed = other.attackSpeed;
		moveSpeed = other.moveSpeed;
	}
	
	public bool Compare(CharacterStats other) {
		return
			attack == other.attack &&
			defense == other.defense && 
			block == other.block && 
			cooldownReduction == other.cooldownReduction && 
			attackSpeed == other.attackSpeed && 
			moveSpeed == other.moveSpeed;
	}
	
	public void ApplyOffset(int val) {
		attack += val;
		defense += val;
		block += val;
		cooldownReduction += val;
		attackSpeed += val;
		moveSpeed += val;
	}
	
	public void ApplyOffset(CharacterStats stats) {
		attack += stats.attack;
		defense += stats.defense;
		block += stats.block;
		cooldownReduction += stats.cooldownReduction;
		attackSpeed += stats.attackSpeed;
		moveSpeed += stats.moveSpeed;
	}
	
	public string multiLineString {
		get {
			var s = "\n";
			return string.Concat(
				"Attack +", attack, s,
				"Defense +", defense, s,
				"Energy +", block, s,
				"CD reduction +", cooldownReduction, s,
				"Attack speed +", attackSpeed, s,
				"Move speed +", moveSpeed
			);
		}
	}
	
	public string GetMultiLineStringCombined(CharacterStats other, string formatStart = "<b>", string formatEnd = "</b>") {
		var s = "\n";
		return string.Concat(
			"Attack: ", attack, " ", formatStart, "+", other.attack, formatEnd, s,
			"Defense: ", defense, " ", formatStart, "+", other.defense, formatEnd, s,
			"Energy: ", block, " ", formatStart, "+", other.block, formatEnd, s,
			"CD reduction: ", cooldownReduction, " ", formatStart, "+", other.cooldownReduction, formatEnd, s,
			"Attack speed: ", attackSpeed, " ", formatStart, "+", other.attackSpeed, formatEnd, s,
			"Move speed: ", moveSpeed, " ", formatStart, "+", other.moveSpeed, formatEnd
		);
	}
	
	public override string ToString() {
		var s = " / ";
		return attack + s + defense + s + block + s + cooldownReduction + s + attackSpeed + s + moveSpeed;
	}
	
	public bool valid {
		get {
			return totalStatPointsUsed <= maxStatPoints;
		}
	}
	
	public int totalStatPointsUsed {
		get {
			return
				attack +
				defense +
				block +
				cooldownReduction + 
				attackSpeed +
				moveSpeed;
		}
	}
	
	public int maxStatPoints {
		get { return 300; }
	}
	
	public int statPointsLeft {
		get { return maxStatPoints - totalStatPointsUsed; }
	}
	
	public float attackDmgMultiplier {
		get { return 1.0f + ((attack - 50) * 0.005f); }
	}
	
	public float defenseDmgMultiplier {
		get { return 1.0f - ((defense - 50) * 0.003333f); }
	}
	
	public float energyMultiplier {
		get {
			return 1.0f + ((block - 50) * 0.0075f);
		}
	}
	
	public float cooldownMultiplier {
		get { return 1.0f - ((cooldownReduction - 50) * 0.005f); }
	}
	
	public float attackSpeedMultiplier {
		get { return 1.0f - ((attackSpeed - 50) * 0.005f); }
	}
	
	public float moveSpeedMultiplier {
		get { return 1.0f + ((moveSpeed - 50) * 0.005f); }
	}
}
