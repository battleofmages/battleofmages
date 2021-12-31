using BoM.Players;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoM.Tests {
	public class ProxyMovementTest {
		struct CalculatePositionData {
			public Vector3 Position;
			public Vector3 Direction;
			public float Speed;
			public float Latency;
			public Vector3 Result;

			public CalculatePositionData(Vector3 position, Vector3 direction, float speed, float latency, Vector3 result) {
				Position = position;
				Direction = direction;
				Speed = speed;
				Latency = latency;
				Result = result;
			}
		};

		[Test]
		public void ProxyMovement_CalculatePosition() {
			var tests = new CalculatePositionData[]{
				new CalculatePositionData(Vector3.zero, Vector3.forward, 1f, 1f, new Vector3(0f, 0f, 1f)),
				new CalculatePositionData(Vector3.zero, Vector3.forward, 4f, 1f, new Vector3(0f, 0f, 4f)),
				new CalculatePositionData(Vector3.zero, Vector3.forward, 4f, 0.1f, new Vector3(0f, 0f, 0.4f))
			};

			foreach(var test in tests) {
				Assert.AreEqual(test.Result, ProxyMovement.CalculatePosition(test.Position, test.Direction, test.Speed, test.Latency));
			}
		}
	}
}
