public class ExperienceEntry : JsonSerializable<ExperienceEntry> {
	public uint experience;
	
	// Empty constructor
	public ExperienceEntry() {
		
	}
	
	// Constructor
	public ExperienceEntry(uint nExperience) {
		experience = nExperience;
	}
}