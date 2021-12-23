using System;
using Unity.Netcode;

namespace BoM.Players {
	public class Visibility : NetworkBehaviour {
		public event Action BecameVisible;
		public event Action BecameInvisible;

		private void OnBecameVisible() {
			BecameVisible?.Invoke();
		}

		private void OnBecameInvisible() {
			BecameInvisible?.Invoke();
		}
	}
}