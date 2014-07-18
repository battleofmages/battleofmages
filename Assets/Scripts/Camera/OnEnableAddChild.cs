using UnityEngine;

public class OnEnableAddChild : MonoBehaviour {
	public Transform child;

	// OnEnable
	void OnEnable() {
		child.parent = transform;
		child.localPosition = Cache.vector3Zero;
		child.localRotation = Cache.quaternionIdentity;
	}
}
