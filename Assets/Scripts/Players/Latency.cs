using System;
using Unity.Netcode;
using UnityEngine;
using System.Collections;

namespace BoM.Players {
	// Data
	public class LatencyData : NetworkBehaviour {
		public float maxRoundTripTime;
		public float oneWay { get; protected set; }
		public int oneWayInMilliseconds { get; protected set; }
		public NetworkVariable<float> roundTripTime;
		protected long startTime;
		protected ClientRpcParams toOwner;
		protected bool waitingForResponse;
	}

	// Logic
	public class Latency : LatencyData {
		public event Action<float> Received;

		private void Awake() {
			roundTripTime.OnValueChanged += OnRoundTripTimeChanged;
		}

		private void OnRoundTripTimeChanged(float oldRTT, float newRTT) {
			// This is not 100% correct because send and receive latency can be different,
			// however it provides a decent approximation.
			oneWay = newRTT * 0.5f;
			oneWayInMilliseconds = (int) (oneWay * 1000f);
			Received?.Invoke(oneWay);
		}

		public override void OnNetworkSpawn() {
			// Initiate network variables
			OnRoundTripTimeChanged(0, roundTripTime.Value);

			if(IsServer) {
				toOwner = new ClientRpcParams {
					Send = new ClientRpcSendParams {
						TargetClientIds = new ulong[] { OwnerClientId }
					}
				};

				StartCoroutine(SendPingsPeriodically());
			}
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
			startTime = Now();
			PingClientRpc(toOwner);
		}

		[ClientRpc]
		public void PingClientRpc(ClientRpcParams rpcParams = default) {
			PongServerRpc();
		}

		[ServerRpc]
		public void PongServerRpc() {
			var now = Now();
			var rtt = (now - startTime) * 0.001f;

			// Prevent cheats that are based on faking your latency
			// for lag compensation by limiting the latency.
			if(rtt > maxRoundTripTime) {
				rtt = maxRoundTripTime;
			}

			roundTripTime.Value = rtt;
			waitingForResponse = false;
		}
	}
}
