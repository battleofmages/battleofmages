using BoM.Core;
using UnityEngine;

namespace BoM.Players.IK {
	public class FootPlacement : Behaviour {
		public Skeleton skeleton;
		public Animator animator;
		public LayerMask layerMask;
		public bool useBonePosition = true;
		[Range(0f, 1f)] public float raycastStartY = 0.1f;
		[Range(0f, 1f)] public float raycastDistanceBelowFeet = 0.3f;
		[Range(0f, 1f)] public float distanceToGround;

		public override void OnAnimatorIK(int layerIndex) {
			UpdateFoot(AvatarIKGoal.LeftFoot, skeleton.leftFoot, "IKLeftFootWeight");
			UpdateFoot(AvatarIKGoal.RightFoot, skeleton.rightFoot, "IKRightFootWeight");
		}

		private void UpdateFoot(AvatarIKGoal foot, Transform bone, string weightParameter) {
			var weight = animator.GetFloat(weightParameter);

			if(weight < 0.1f) {
				return;
			}

			Vector3 footPosition;

			if(useBonePosition) {
				footPosition = bone.position;
			} else {
				footPosition = animator.GetIKPosition(foot);
			}

			if(!Physics.Raycast(footPosition + new Vector3(0f, raycastStartY, 0f), Const.DownVector, out RaycastHit hit, raycastStartY + raycastDistanceBelowFeet, layerMask)) {
				return;
			}

			animator.SetIKPositionWeight(foot, weight);
			animator.SetIKRotationWeight(foot, weight);

			Vector3 axis = Vector3.Cross(Const.UpVector, hit.normal);
			float angle = Vector3.Angle(Const.UpVector, hit.normal);
			Quaternion newRotation = Quaternion.AngleAxis(angle, axis);
			Quaternion footRotation = animator.GetIKRotation(foot);

			var ikPosition = hit.point + new Vector3(0f, distanceToGround, 0f);
			var ikRotation = newRotation * Quaternion.AngleAxis(footRotation.eulerAngles.y, Const.UpVector);

			animator.SetIKPosition(foot, ikPosition);
			animator.SetIKRotation(foot, ikRotation);
		}
	}
}
