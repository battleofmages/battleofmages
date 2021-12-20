#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace BoM.Editor {
	public class SnapToGround : MonoBehaviour {
		[MenuItem("Custom/Snap To Ground %g")]
		public static void SnapGround() {
			foreach(var transform in Selection.transforms) {
				var hits = Physics.RaycastAll(transform.position + Vector3.up, Vector3.down);
				var highest = -Mathf.Infinity;
				var count = 0;

				foreach(var hit in hits) {
					if(hit.collider.gameObject == transform.gameObject) {
						continue;
					}

					if(hit.point.y > highest) {
						highest = hit.point.y;
					}

					count++;
				}

				if(count == 0) {
					return;
				}

				Undo.RecordObject(transform, "Snap to ground");
				transform.position = new Vector3(transform.position.x, highest, transform.position.z);
			}
		}
	}
}
#endif
