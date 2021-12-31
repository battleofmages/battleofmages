using BoM.Core;
using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	public class Entity : NetworkBehaviour, IPlayer {
		public Latency Latency;
		public ulong ClientId { get; set; }
		public int Ping { get { return Latency.oneWayInMilliseconds; } }
		public string Nick { get { return gameObject.name; } }
		public GameObject GameObject { get { return gameObject; } }
		public Transform Transform { get { return transform; } }
		public Vector3 RemotePosition { get; set; }
		public Vector3 RemoteDirection { get; set; }

		public Health health;
		public Energy energy;
		public Rotation rotation;
		public Cameras.Center camCenter;

		protected NetworkVariable<int> teamId = new NetworkVariable<int>();

		public int TeamId {
			get { return teamId.Value; }
			set { teamId.Value = value; }
		}

		public void Respawn(Vector3 newPosition, Quaternion newRotation) {
			// Position
			transform.position = newPosition;
			RemotePosition = newPosition;
			RemoteDirection = Const.ZeroVector;
			Physics.SyncTransforms();

			// Character rotation
			rotation.SetRotation(newRotation);

			// Camera rotation
			camCenter.SetRotation(newRotation);

			if(IsServer) {
				RespawnClientRpc(newPosition, newRotation);
				health.Reset();
				energy.Reset();
			}
		}

		[ClientRpc]
		public void RespawnClientRpc(Vector3 newPosition, Quaternion newRotation) {
			if(IsHost) {
				return;
			}

			Respawn(newPosition, newRotation);
		}
	}
}
