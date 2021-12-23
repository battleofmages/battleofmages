using UnityEngine;
using System.Collections.Generic;

namespace BoM.Teams {
	public class Manager : MonoBehaviour {
		public static Manager Instance { get; private set; }
		public List<Team> teams;
		
		private void Awake() {
			Instance = this;
		}

		public static List<Team> Teams {
			get {
				return Instance.teams;
			}
		}
	}
}
