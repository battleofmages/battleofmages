[System.Serializable]
public class ArtifactInventory : InventorySerializer<ArtifactInventory>, Inventory {
	public static int defaultInventoryLimit = 15;

	// Bags
	public Bag[] bags { get; set; }

	// Constructor
	public ArtifactInventory() {
		bags = new Bag[Artifact.maxLevel];
		
		for(int i = 0; i < Artifact.maxLevel; i++) {
			bags[i] = new Bag(defaultInventoryLimit);
		}
	}

	// AddArtifact
	public void AddArtifact(Artifact arti) {
		bags[arti.level].AddItem(arti, 1);
	}

	// RemoveArtifact
	public void RemoveArtifact(Artifact arti) {
		bags[arti.level].RemoveItem(arti.id, 1);
	}
}
