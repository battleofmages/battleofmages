using BoM.Core;
using BoM.Skills;
using BoM.Network;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BoM.Players {
	// Data
	public class HealthData : NetworkBehaviour {
		[SerializeField] protected NetworkVariable<float> health;
		[SerializeField] protected NetworkVariable<float> maxHealth;
		[SerializeField] protected Skills.Manager skills;
		[SerializeField] protected Player player;

		protected List<DamageEvent> damageEvents;
	}

	// Logic
	public class Health : HealthData, IHealth {
		public event Action<DamageEvent> Damaged;
		public event Action<DamageEvent> Died;
		public event Action Revived;
		public event Action<float> Changed;
		public event Action<float> PercentChanged;

		public bool isAlive { get => health.Value > 0f; }
		public bool isDead { get => !isAlive; }

		private void Awake() {
			damageEvents = new List<DamageEvent>();

			health.OnValueChanged += (oldHealth, newHealth) => {
				Changed?.Invoke(newHealth);
				PercentChanged?.Invoke(newHealth / maxHealth.Value);

				if(oldHealth <= 0f && newHealth > 0f) {
					Revived?.Invoke();
				}
			};
		}

		public override void OnNetworkSpawn() {
			// if(IsServer) {
			// 	maxHealth.Value = 200f;
			// }

			if(IsClient) {
				Changed?.Invoke(health.Value);
				PercentChanged?.Invoke(health.Value / maxHealth.Value);
			}
		}

		public void Reset() {
			health.Value = maxHealth.Value;
		}

		public void TakeDamage(float damage, ISkill skill, IPlayer caster) {
			if(!IsServer || isDead) {
				return;
			}

			var damageEvent = new DamageEvent(player, damage, skill, caster);
			damageEvents.Add(damageEvent);
			Damaged?.Invoke(damageEvent);

			bool wasAlive = isAlive;
			bool isKillingBlow = false;

			health.Value -= damage;

			if(wasAlive && health.Value <= 0f) {
				health.Value = 0f;
				Died?.Invoke(damageEvent);
				isKillingBlow = true;
			}

			TakeDamageClientRpc(damage, skill.Id, caster.ClientId, isKillingBlow);
		}

		[ClientRpc]
		public void TakeDamageClientRpc(float damage, short skillId, ulong casterClientId, bool isKillingBlow) {
			if(IsHost) {
				return;
			}

			var caster = PlayerManager.GetByClientId(casterClientId);
			var skill = skills.GetSkillById(skillId);
			var damageEvent = new DamageEvent(player, damage, skill, caster);
			damageEvents.Add(damageEvent);
			Damaged?.Invoke(damageEvent);

			if(isKillingBlow) {
				Died?.Invoke(damageEvent);
			}
		}
	}
}
