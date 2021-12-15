using UnityEngine;

public interface IPlayer {
	string Name { get; }
	ulong ClientId { get; }
	Vector3 RemotePosition { get; set; }
	Vector3 RemoteDirection { get; set; }
}