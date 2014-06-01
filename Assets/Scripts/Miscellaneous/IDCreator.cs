using UnityEngine;

public class IDCreator {
	public const ushort UndefinedId = ushort.MaxValue;
	private static ushort nextId;

	// GetNextID
	public static ushort GetNextID() {
		var id = IDCreator.nextId++;
		
		if(id == IDCreator.UndefinedId)
			id = IDCreator.nextId++;

		return id;
	}
}
