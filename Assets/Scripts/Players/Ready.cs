using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace BoM.Players {
	// Data
	public class ReadyData : NetworkBehaviour {
		[SerializeField] protected NetworkVariable<bool> isReady;
	}

	// Logic
	public class Ready : ReadyData {
		private IEnumerator Start() {
			yield return new WaitForEndOfFrame();
			ReadyServerRpc();
		}

		[ServerRpc]
		public void ReadyServerRpc() {
			isReady.Value = true;
		}
	}
}
