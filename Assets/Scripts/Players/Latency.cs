using System;
using Unity.Netcode;
using UnityEngine;
using System.Collections;

namespace BoM.Players {
	public class Latency : NetworkBehaviour {
		public event Action<long> Received;
		public long latency { get; private set; }
		private long roundTripTime;
		private long pingTime;
		private ClientRpcParams toOwner;

		private IEnumerator Start() {
			if(!IsServer) {
				yield break;
			}

			toOwner = new ClientRpcParams {
				Send = new ClientRpcSendParams {
					TargetClientIds = new ulong[]{ OwnerClientId }
				}
			};

			while(true) {
				Ping();
				yield return new WaitForSecondsRealtime(1f);
			}
		}

		private void Ping() {
			pingTime = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
			PingClientRpc(pingTime, toOwner);
		}

		[ClientRpc]
		public void PingClientRpc(long serverNow, ClientRpcParams rpcParams = default) {
			if(!IsOwner) {
				return;
			}

			var now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			latency = now - serverNow;
			PongServerRpc();
		}

		[ServerRpc]
		public void PongServerRpc() {
			var now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			roundTripTime = now - pingTime;
			ReceiveRTTClientRpc(roundTripTime);

			// This is not 100% correct because send and receive latency can be different,
			// however it provides a decent approximation.
			latency = roundTripTime / 2;
		}

		[ClientRpc]
		public void ReceiveRTTClientRpc(long rtt) {
			roundTripTime = rtt;

			// This is not 100% correct because send and receive latency can be different,
			// however it provides a decent approximation.
			latency = roundTripTime / 2;
			
			if(IsOwner) {
				Received?.Invoke(latency);
			}
		}
	}
}