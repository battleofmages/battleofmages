using UnityEngine;

namespace BoM.Core {
	public interface IPlayer {
		ulong ClientId { get; }
		int Ping { get; }
		string Nick { get; }
		Vector3 RemotePosition { get; set; }
		Vector3 RemoteDirection { get; set; }
	}
}
