[System.Serializable]
public class ArtifactSlot {
	public Artifact _artifact;
	public byte requiredLevel;

	// Empty constructor
	public ArtifactSlot() {
		_artifact = null;
		requiredLevel = 0;
	}

	// Constructor
	public ArtifactSlot(byte nRequiredLevel) {
		_artifact = null;
		requiredLevel = nRequiredLevel;
	}

	// Artifact
	public Artifact artifact {
		get {
			return _artifact;
		}

		set {
			if(value == null || requiredLevel == value.level) {
				_artifact = value;
			} else {
				LogManager.General.LogWarning("Cannot assign a level " + value.level + " artifact to a level " + requiredLevel + " slot.");
			}
		}
	}
	
	// Writer
	public static void JsonSerializer(Jboy.JsonWriter writer, object instance) {
		var slot = (ArtifactSlot)instance;
		
		writer.WriteObjectStart();
		
		writer.WritePropertyName("requiredLevel");
		writer.WriteNumber(slot.requiredLevel);
		
		writer.WritePropertyName("artifactId");
		if(slot.artifact != null)
			writer.WriteNumber(slot.artifact.id);
		else
			writer.WriteNumber(-1);
		
		writer.WriteObjectEnd();
	}
	
	// Reader
	public static object JsonDeserializer(Jboy.JsonReader reader) {
		var slot = new ArtifactSlot();
		
		reader.ReadObjectStart();
		
		reader.ReadPropertyName("requiredLevel");
		slot.requiredLevel = (byte)reader.ReadNumber();
		
		reader.ReadPropertyName("artifactId");
		int itemId = (int)reader.ReadNumber();
		
		if(itemId != -1) {
			slot.artifact = new Artifact(itemId);
		}
		
		reader.ReadObjectEnd();
		
		return slot;
	}
}
