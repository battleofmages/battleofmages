using UnityEngine;

namespace BoM.Players {
	[CreateAssetMenu(fileName = "Build", menuName = "BoM/Build", order = 30)]
	public class Build : ScriptableObject {
		public Skills.Bar[] Elements;
		public Traits Traits;
	}
}
