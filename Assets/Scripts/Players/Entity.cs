using BoM.Core;
using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	public class Entity : NetworkBehaviour, IPlayer {
		public NetworkVariable<int> teamId;
		public Latency latency;

		public ulong ClientId { get; set; }
		public int Ping { get { return latency.oneWayInMilliseconds; } }
		public string Nick { get { return gameObject.name; } }
		public GameObject GameObject { get { return gameObject; } }
		public Transform Transform { get { return transform; } }
		public Vector3 RemotePosition { get; set; }
		public Vector3 RemoteDirection { get; set; }

		public int TeamId {
			get { return teamId.Value; }
			set { teamId.Value = value; }
		}
	}
}
