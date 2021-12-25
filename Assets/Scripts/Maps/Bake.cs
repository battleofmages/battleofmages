// Source:
// https://forum.unity.com/threads/542440/#post-7752681

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Netcode;

namespace BoM.Maps {
	public class Bake : MonoBehaviour {
		private ReflectionProbe baker;

		private void Start() {
			if(!NetworkManager.Singleton) {
				return;
			}

			baker = gameObject.AddComponent<ReflectionProbe>();
			baker.cullingMask = 0;
			baker.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
			baker.mode = ReflectionProbeMode.Realtime;
			baker.timeSlicingMode = ReflectionProbeTimeSlicingMode.NoTimeSlicing;

			RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
			StartCoroutine(UpdateEnvironment());
		}

		private IEnumerator UpdateEnvironment() {
			DynamicGI.UpdateEnvironment();
			baker.RenderProbe();
			yield return new WaitForEndOfFrame();
			RenderSettings.customReflectionTexture = baker.texture;
		}
	}
}
