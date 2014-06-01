public static class ItemFactory {
	// CreateFromId
	public static Item CreateFromId(int id) {
		if(id >= 0 && id <= 1079)
			return new Artifact(id);

		return null;
	}
}