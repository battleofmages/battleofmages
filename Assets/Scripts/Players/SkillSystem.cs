using BoM.Core;
using BoM.Skills;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BoM.Players {
	public class SkillSystem : NetworkBehaviour {
		public Player player;
		public Cursor cursor;
		public Skeleton skeleton;
		public Animations animations;
		public List<Element> elements { get; set; }
		private int currentElementIndex;
		private float time;

		const float baseCastTime = 0.4f;
		const float animationTime = 0.6f;

		public Element currentElement {
			get {
				return elements[currentElementIndex];
			}
		}

		public bool isCasting {
			get {
				return time < 1f;
			}
		}

		private void Awake() {
			time = 1f;
		}

		public void UseSkill(Skill skill, Vector3 cursor) {
			Vector3 skillPosition = Vector3.zero;
			Quaternion skillRotation = Quaternion.identity;

			switch(skill.position) {
				case PositionType.Cursor:
					skillPosition = cursor;
					break;

				case PositionType.Hands:
					skillPosition = skeleton.handsCenter;
					break;
			}

			switch(skill.rotation) {
				case RotationType.ToCursor:
					var towardsCursor = cursor - skillPosition;

					if(towardsCursor != Vector3.zero) {
						skillRotation = Quaternion.LookRotation(towardsCursor);
					}

					break;
			}

			var instance = skill.pool.Get();
			instance.skill = skill;
			instance.caster = player;
			instance.transform.SetLayer(gameObject.layer);
			instance.transform.position = skillPosition;
			instance.transform.rotation = skillRotation;
			instance.pool = skill.pool;
			instance.Init();
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

			var waitTime = (int)((castTime - startTime) * 1000f);

			if(waitTime > 0) {
				await Task.Delay(waitTime);
			}

			onFinish();
		}

		public void CastSkillAtIndex(byte slotIndex) {
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
			if(IsOwner || IsServer) {
				return;
			}

			Cast(
				Player.main.latency.oneWay,
				baseCastTime,
				() => UseSkill(currentElement.skills[index], remoteCursorPosition)
			);
		}
	}
}