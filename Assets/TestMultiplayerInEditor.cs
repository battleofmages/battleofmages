#if UNITY_EDITOR
using UnityEngine;
using ParrelSync;

public class TestMultiplayerInEditor: MonoBehaviour {
	public Network network;

	void Start() {
		if(ClonesManager.IsClone()) {
			network.StartClient();
		} else {
			network.StartHost();
		}
	}
}
#endif