using System;
using Unity.Netcode;

namespace BoM.Players {
	public class Latency : NetworkBehaviour {
		public event Action<long, long> Received;
		private long latencyIn;
		private long latencyOut;

		private void Start() {
			if(!IsOwner) {
				return;
			}

			InvokeRepeating("SendPing", 0f, 1f);
		}

		private void SendPing() {
			var now = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
			PingServerRpc(now);
		}

		[ServerRpc]
		public void PingServerRpc(long clientNow) {
			var now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			latencyOut = now - clientNow;
			PongClientRpc(now, latencyOut);
		}

		[ClientRpc]
		public void PongClientRpc(long serverNow, long latencyOut) {
			this.latencyOut = latencyOut;

			if(!IsOwner) {
				return;
			}

			var now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			latencyIn = now - serverNow;
			Received?.Invoke(latencyIn, this.latencyOut);
		}
	}
}