public class Rune {
	public static int maxLevel = 5;
	
	public Skill detonationSkill;
	private int _level = 0;

	// Constructor
	public Rune(Skill nDetonationSkill) {
		detonationSkill = nDetonationSkill;
	}
	
	// Current level of the rune
	public int level {
		get {
			return _level;
		}
		
		set {
			if(value < 0)
				_level = 0;
			else if(value > maxLevel)
				_level = maxLevel;
			else
				_level = value;
		}
	}
}
