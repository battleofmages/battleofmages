using UnityEngine;

namespace BoM.Players {
	// Data
	public class ModelData : MonoBehaviour {
		[SerializeField] protected Transform model;
		[SerializeField] protected float modelYOffset;
		[SerializeField] protected CharacterController controller;
	}

	// Logic
	public class Model : ModelData {
		private void Awake() {
			var modelRenderer = model.GetComponentInChildren<SkinnedMeshRenderer>();
			modelRenderer.gameObject.AddComponent<Visibility>();
			model.localPosition = new Vector3(0f, model.localPosition.y - controller.skinWidth + modelYOffset, 0f);
		}
	}
}
