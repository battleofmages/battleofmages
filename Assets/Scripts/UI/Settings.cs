using BoM.Core;
using UnityEngine;

namespace BoM.UI {
	[CreateAssetMenu(fileName = "UI Settings", menuName = "BoM/UI Settings", order = 200)]
	public class Settings : Singleton<Settings> {
		[SerializeField]
		private float fadeDuration;

		public static float FadeDuration { get { return Instance.fadeDuration; } }
	}
}
