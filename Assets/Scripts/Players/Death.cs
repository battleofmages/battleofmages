using Unity.Netcode;
using UnityEngine;

namespace BoM.Players {
	public class Death : NetworkBehaviour {
		public MonoBehaviour[] components;
		public GameObject[] objects;
		public Collider[] colliders;
		public OwnerMovement ownerMovement;
		public ProxyMovement proxyMovement;
		public Health health;

		private void Awake() {
			health.Died += OnDeath;
			health.Revived += OnRevive;
		}

		private void OnDeath(DamageEvent damageEvent) {
			Reset(false);
		}

		private void OnRevive() {
			Reset(true);
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
