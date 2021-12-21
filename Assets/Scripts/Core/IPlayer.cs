using UnityEngine;

namespace BoM.Core {
	public interface IPlayer {
		ulong ClientId { get; }
		float Latency { get; }
		string Nick { get; }
		Vector3 RemotePosition { get; set; }
		Vector3 RemoteDirection { get; set; }
	}
}
