#if UNITY_EDITOR
using UnityEngine;
using ParrelSync;

namespace BoM.Testing {
	public class EditorTest : MonoBehaviour {
		private void Start() {
			if(ClonesManager.IsClone()) {
				Network.Client.Start();
			} else {
				Network.Server.Start();
			}
		}
	}
}
#endif