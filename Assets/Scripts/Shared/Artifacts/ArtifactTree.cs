[System.Serializable]
public class ArtifactTree {
	public ArtifactSlot[][] slots;

	// Constructor
	public ArtifactTree() {
		slots = new ArtifactSlot[5][];
		
		for(byte i = 0; i < Artifact.maxLevel; i++) {
			int numSlots = Artifact.maxLevel - i;
			slots[i] = new ArtifactSlot[numSlots];
			
			for(int slotIndex = 0; slotIndex < numSlots; slotIndex++) {
				slots[i][slotIndex] = new ArtifactSlot(i);
			}
		}
	}

	// Char stats
	public CharacterStats charStats {
		get {
			CharacterStats stats = new CharacterStats(0);
			
			foreach(var slotLevel in this.slots) {
				foreach(var slot in slotLevel) {
					if(slot.artifact != null) {
						stats.ApplyOffset(slot.artifact.charStats);
					}
				}
			}
			
			return stats;
		}
	}

	// AddArtifact
	public bool AddArtifact(int itemId) {
		var arti = new Artifact(itemId);
		var slotLevel = slots[arti.level];
		
		for(int i = 0; i < slotLevel.Length; i++) {
			if(slotLevel[i].artifact == null) {
				slotLevel[i].artifact = arti;
				return true;
			}
		}
		
		return false;
	}

	// GetStarterArtifactTree
	public static ArtifactTree GetStarterArtifactTree() {
		var tree = new ArtifactTree();
		tree.BuildStarterArtifacts();
		return tree;
	}

	// BuildStarterArtifacts
	public void BuildStarterArtifacts() {
		bool otherHalf = false;
		
		foreach(var slotLevel in slots) {
			foreach(var slot in slotLevel) {
				slot.artifact = new Artifact(slot.requiredLevel);
				slot.artifact.stats[0] = otherHalf ? Artifact.Stat.Attack : Artifact.Stat.Defense;
				slot.artifact.stats[1] = otherHalf ? Artifact.Stat.Energy : Artifact.Stat.AttackSpeed;
				slot.artifact.stats[2] = otherHalf ? Artifact.Stat.MoveSpeed : Artifact.Stat.CooldownReduction;
				
				otherHalf = !otherHalf;
			}
		}
	}

	// Randomize
	public void Randomize() {
		foreach(var slotLevel in slots) {
			foreach(var slot in slotLevel) {
				slot.artifact = new Artifact(slot.requiredLevel);
			}
		}
	}
	
	// Writer
	public static void JsonSerializer(Jboy.JsonWriter writer, object instance) {
		writer.WriteArrayStart();
		
		var tree = (ArtifactTree)instance;
		foreach(var slotLevel in tree.slots) {
			writer.WriteArrayStart();
			
			for(int i = 0; i < slotLevel.Length; i++) {
				Jboy.Json.WriteObject(slotLevel[i].artifact, writer);
			}
			
			writer.WriteArrayEnd();
		}
		
		writer.WriteArrayEnd();
	}
	
	// Reader
	public static object JsonDeserializer(Jboy.JsonReader reader) {
		reader.ReadArrayStart();
		
		var tree = new ArtifactTree();
		for(int i = 0; i < tree.slots.Length; i++) {
			int numSlots = Artifact.maxLevel - i;
			var slotLevel = tree.slots[i];
			reader.ReadArrayStart();
			
			for(int slotIndex = 0; slotIndex < numSlots; slotIndex++) {
				var artifactSlot = new ArtifactSlot((byte)i);
				artifactSlot.artifact = Jboy.Json.ReadObject<Artifact>(reader);
				
				slotLevel[slotIndex] = artifactSlot;
			}
			
			reader.ReadArrayEnd();
		}
		
		reader.ReadArrayEnd();
		return tree;
	}
}
