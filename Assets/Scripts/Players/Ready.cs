using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace BoM.Players {
	public class Ready : NetworkBehaviour {
		public NetworkVariable<bool> isReady;

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
