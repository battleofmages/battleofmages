using BoM.Core;
using Unity.Netcode;
using UnityEngine;

namespace BoM.Players {
	// Data
	public class DeathData : NetworkBehaviour {
		[SerializeField] protected Player player;
		[SerializeField] protected Rotation rotation;
		[SerializeField] protected float respawnTime;
		[SerializeField] protected MonoBehaviour[] components;
		[SerializeField] protected GameObject[] objects;
		[SerializeField] protected Collider[] colliders;
		[SerializeField] protected Health health;
		[SerializeField] protected OwnerMovement ownerMovement;
		[SerializeField] protected ProxyMovement proxyMovement;
	}

	// Logic
	public class Death : DeathData {
		private void Awake() {
			health.Died += OnDeath;
			health.Revived += OnRevive;
		}

		private void OnDeath(DamageEvent damageEvent) {
			Reset(false);
			transform.SetLayer(Const.SpectatorLayer);
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
