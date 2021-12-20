using System;
using Unity.Netcode;
using UnityEngine;
using System.Collections;

namespace BoM.Players {
	public class Latency : NetworkBehaviour {
		public event Action<long> Received;
		public long oneWay { get; private set; }
		private long roundTripTime;
		private long pingSentTime;
		private ClientRpcParams toOwner;
		private bool waitingForResponse;

		public override void OnNetworkSpawn() {
			if(!IsServer) {
				return;
			}

			toOwner = new ClientRpcParams {
				Send = new ClientRpcSendParams {
					TargetClientIds = new ulong[]{ OwnerClientId }
				}
			};

			StartCoroutine(SendPingsPeriodically());
		}

		private IEnumerator SendPingsPeriodically() {
			while(true) {
				if(!waitingForResponse) {
					Ping();
				}
				
				yield return new WaitForSecondsRealtime(1f);
			}
		}

		private long Now() {
			return System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
		}

		private void Ping() {
			waitingForResponse = true;
			pingSentTime = Now();
			PingClientRpc(pingSentTime, toOwner);
		}

		[ClientRpc]
		public void PingClientRpc(long serverNow, ClientRpcParams rpcParams = default) {
			var now = Now();
			oneWay = now - serverNow;
			PongServerRpc();
		}

		[ServerRpc]
		public void PongServerRpc() {
			var now = Now();
			roundTripTime = now - pingSentTime;
			ReceiveRTTClientRpc(roundTripTime);
			waitingForResponse = false;

			// This is not 100% correct because send and receive latency can be different,
			// however it provides a decent approximation.
			oneWay = roundTripTime / 2;
		}

		[ClientRpc]
		public void ReceiveRTTClientRpc(long rtt) {
			roundTripTime = rtt;

			// This is not 100% correct because send and receive latency can be different,
			// however it provides a decent approximation.
			oneWay = roundTripTime / 2;
			
			if(IsOwner) {
				Received?.Invoke(oneWay);
			}
		}
	}
}