using BoM.Network;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

namespace BoM.Maps {
	// Data
	public class MapData : MonoBehaviour {
		[SerializeField] protected Server server;
		[SerializeField] protected GameObject[] deactivateOnPlay;
		protected ReflectionProbe baker;
	}

	// Logic
	public class Map : MapData {
		private void Start() {
			if(!NetworkManager.Singleton) {
				server.MapName = SceneManager.GetActiveScene().name;
				SceneManager.LoadScene("Main");
				return;
			}

			foreach(var obj in deactivateOnPlay) {
				obj.SetActive(false);
			}
		}
	}
}
