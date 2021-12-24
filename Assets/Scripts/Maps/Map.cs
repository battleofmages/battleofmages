using BoM.Network;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

namespace BoM.Maps {
	public class Map : MonoBehaviour {
		public GameObject[] deactivateOnPlay;
		private ReflectionProbe baker;

		private void Start() {
			if(!NetworkManager.Singleton) {
				Server.mapName = SceneManager.GetActiveScene().name;
				SceneManager.LoadScene("Main");
				return;
			}

			foreach(var obj in deactivateOnPlay) {
				obj.SetActive(false);
			}
		}
	}
}
