using UnityEngine;

namespace BoM.Core {
	public interface IPlayer {
		Account Account { get; }
		ulong ClientId { get; }
		float Latency { get; }
		Vector3 RemotePosition { get; set; }
		Vector3 RemoteDirection { get; set; }
	}
}
