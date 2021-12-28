using UnityEngine;

namespace BoM.Core {
	public static class Const {
		public static readonly Vector3 ZeroVector = new Vector3(0f, 0f, 0f);
		public static readonly Vector3 UpVector = new Vector3(0f, 1f, 0f);
		public static readonly Vector3 DownVector = new Vector3(0f, -1f, 0f);
		public static readonly Quaternion NoRotation = new Quaternion(0f, 0f, 0f, 1f);
	}
}
