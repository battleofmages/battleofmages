#if UNITY_EDITOR
using UnityEngine;
using ParrelSync;

namespace BoM {
	public class EditorTest : MonoBehaviour {
		private void Start() {
			if(ClonesManager.IsClone()) {
				Network.Client.Start();
			} else {
				Network.Host.Start();
			}
		}
	}
}
#endif