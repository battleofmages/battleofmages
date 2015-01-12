using UnityEngine;

public class OnlineStatusSprites : SingletonMonoBehaviour<OnlineStatusSprites> {
	public Sprite[] sprites;

	// Get
	public static Sprite Get(OnlineStatus status) {
		return OnlineStatusSprites.instance.sprites[(int)status];
	}
}
