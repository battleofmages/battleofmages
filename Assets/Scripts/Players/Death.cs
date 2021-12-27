using Unity.Netcode;
using UnityEngine;

namespace BoM.Players {
	public class Death : NetworkBehaviour {
		public Player player;
		public float respawnTime;
		public MonoBehaviour[] components;
		public GameObject[] objects;
		public Collider[] colliders;
		public OwnerMovement ownerMovement;
		public ProxyMovement proxyMovement;
		public Flight flight;
		public Health health;

		private void Awake() {
			health.Died += OnDeath;
			health.Revived += OnRevive;
		}

		private void OnDeath(DamageEvent damageEvent) {
			Reset(false);

			if(IsServer) {
				flight.isActive.Value = false;
			} else {
				flight.enabled = false;
			}

			if(IsServer) {
				Invoke("Respawn", respawnTime);
			}
		}

		private void OnRevive() {
			Reset(true);
		}

		private void Respawn() {
			var team = player.Team;
			var spawnPosition = team.SpawnPosition;
			var spawnRotation = team.SpawnRotation;

			transform.SetPositionAndRotation(spawnPosition, spawnRotation);
			RespawnClientRpc(spawnPosition, spawnRotation);

			health.health.Value = health.maxHealth.Value;
		}

		[ClientRpc]
		public void RespawnClientRpc(Vector3 position, Quaternion rotation) {
			transform.SetPositionAndRotation(position, rotation);
			player.camCenter.SetRotation(rotation);
		}

		private void Reset(bool state) {
			foreach(var collider in colliders) {
				collider.enabled = state;
			}

			foreach(var component in components) {
				component.enabled = state;
			}

			foreach(var gameObject in objects) {
				gameObject.SetActive(state);
			}

			if(IsOwner) {
				ownerMovement.enabled = state;
			} else {
				proxyMovement.enabled = state;
			}
		}
	}
}
