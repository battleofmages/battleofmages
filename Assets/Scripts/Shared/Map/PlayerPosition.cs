using UnityEngine;

public class PlayerPosition : JsonSerializable<PlayerPosition> {
	public float x;
	public float y;
	public float z;

	// Empty constructor
	public PlayerPosition() {

	}

	// Constructor
	public PlayerPosition(Vector3 vec) {
		x = vec.x;
		y = vec.y;
		z = vec.z;
	}

	// ToVector3
	public Vector3 ToVector3() {
		return new Vector3(x, y, z);
	}

	// ToString
	public override string ToString() {
		return string.Format("{0}, {1}, {2}", x, y, z);
	}
}
