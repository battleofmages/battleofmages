using BoM.Core;
using Unity.Netcode;
using UnityEngine;

namespace BoM.Players {
	public class Death : NetworkBehaviour {
		[SerializeField] private Player player;
		[SerializeField] private Rotation rotation;
		[SerializeField] private float respawnTime;
		[SerializeField] private MonoBehaviour[] components;
		[SerializeField] private GameObject[] objects;
		[SerializeField] private Collider[] colliders;
		[SerializeField] private Flight flight;
		[SerializeField] private Health health;
		[SerializeField] private OwnerMovement ownerMovement;
		[SerializeField] private ProxyMovement proxyMovement;

		private void Awake() {
			health.Died += OnDeath;
			health.Revived += OnRevive;
		}

		private void OnDeath(DamageEvent damageEvent) {
			Reset(false);
			transform.SetLayer(Const.SpectatorLayer);
			flight.Deactivate();
			rotation.center.eulerAngles = new Vector3(0f, rotation.center.eulerAngles.y, 0f);

			if(IsServer) {
				Invoke(nameof(Respawn), respawnTime);
			}
		}

		private void Respawn() {
			player.Respawn(player.Team.RandomSpawnPosition, player.Team.SpawnRotation);
		}

		private void OnRevive() {
			Reset(true);
			transform.SetLayer(player.Team.layer);
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
		}


	}
}
