using BoM.Core;
using BoM.Skills;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BoM.Players {
	// Data
	public class SkillSystemData : NetworkBehaviour {
		public List<Element> Elements { get; set; }

		[SerializeField] protected Player player;
		[SerializeField] protected Cursor cursor;
		[SerializeField] protected Skeleton skeleton;
		[SerializeField] protected Animations animations;
		[SerializeField] protected Energy energy;

		protected int currentElementIndex;
		protected float time;
	}

	// Logic
	public class SkillSystem : SkillSystemData {
		private const float baseCastTime = 0.4f;
		private const float animationTime = 0.6f;

		public bool isCasting { get => time < 1f; }
		public Element currentElement { get => Elements[currentElementIndex]; }

		private void OnEnable() {
			time = 1f;
		}

		public void UseSkill(Skill skill, Vector3 cursor) {
			Vector3 skillPosition = Const.ZeroVector;
			Quaternion skillRotation = Const.NoRotation;

			switch(skill.position) {
				case PositionType.Cursor:
					skillPosition = cursor;
					break;

				case PositionType.Hands:
					skillPosition = skeleton.HandsCenter;
					break;
			}

			switch(skill.rotation) {
				case RotationType.ToCursor:
					var towardsCursor = cursor - skillPosition;

					if(towardsCursor != Const.ZeroVector) {
						skillRotation = Quaternion.LookRotation(towardsCursor);
					}

					break;
			}

			var instance = skill.pool.Get();
			instance.skill = skill;
			instance.caster = player;
			instance.transform.SetLayer(gameObject.layer);
			instance.transform.SetPositionAndRotation(skillPosition, skillRotation);
			instance.pool = skill.pool;
			instance.Init();

			if(IsServer) {
				energy.Consume(10f);
			}
		}

		private void Update() {
			if(time > 1f) {
				return;
			}

			time += Time.deltaTime;
			animations.Animator.SetFloat("CastProgress", time / animationTime);
		}

		private async void Cast(float startTime, float castTime, System.Action onFinish) {
			time = startTime;
			animations.Animator.SetBool("Attack", true);

			var waitTime = (int) ((castTime - startTime) * 1000f);

			if(waitTime > 0) {
				await Task.Delay(waitTime);
			}

			onFinish();
		}

		public void CastSkillAtIndex(byte slotIndex) {
			if(!enabled) {
				return;
			}

			if(slotIndex < 0 || slotIndex >= currentElement.skills.Count) {
				return;
			}

			var skill = currentElement.skills[slotIndex];

			Cast(
				0f,
				baseCastTime,
				() => UseSkill(skill, cursor.Position)
			);

			CastSkillServerRpc(slotIndex, cursor.Position);
		}

		[ServerRpc]
		public void CastSkillServerRpc(byte index, Vector3 remoteCursorPosition) {
			if(!enabled) {
				return;
			}

			if(IsHost && IsOwner) {
				CastSkillClientRpc(index, remoteCursorPosition);
				return;
			}

			Cast(
				0f,
				baseCastTime,
				() => UseSkill(currentElement.skills[index], remoteCursorPosition)
			);

			CastSkillClientRpc(index, remoteCursorPosition);
		}

		[ClientRpc]
		public void CastSkillClientRpc(byte index, Vector3 remoteCursorPosition) {
			if(!enabled || IsOwner || IsServer) {
				return;
			}

			Cast(
				Player.Main.Latency.OneWay,
				baseCastTime,
				() => UseSkill(currentElement.skills[index], remoteCursorPosition)
			);
		}
	}
}
