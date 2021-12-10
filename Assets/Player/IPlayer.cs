using UnityEngine;

public interface IPlayer {
	string Name { get; }
	ulong ClientId { get; }
	Vector3 Position { get; set; }
}