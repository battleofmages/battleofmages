using BoM.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoM.Teams {
	[Serializable]
	public class Team {
		public byte Id { get; set; }
		public string name;
		[SerializeField, Layer] public int layer;
		private List<IPlayer> players = new List<IPlayer>();
		public Transform spawn { get; private set; }
		public float spawnRadius { get; private set; }

		public int layerMask {
			get {
				return 1 << layer;
			}
		}

		public Vector3 RandomSpawnPosition {
			get {
				var offset = UnityEngine.Random.insideUnitCircle * spawnRadius;
				return new Vector3(spawn.position.x + offset.x, spawn.position.y, spawn.position.z + offset.y);
			}
		}

		public Quaternion SpawnRotation {
			get {
				return spawn.rotation;
			}
		}

		public void AddPlayer(IPlayer player) {
			players.Add(player);
		}

		public void RemovePlayer(IPlayer player) {
			players.Remove(player);
		}

		public void FindSpawn() {
			spawn = GameObject.Find($"Spawn {Id + 1}").transform;
			spawnRadius = spawn.GetComponent<SphereCollider>().radius;
		}
	}
}
