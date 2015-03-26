using UnityEngine;
using System.Collections;

// Singleton
public abstract class Singleton<T> where T : Singleton<T> {
	private static T _instance = default(T);
	
	// Instance
	public static T instance {
		get {
			return _instance;
		}
	}
}

// SingletonMonoBehaviour
public abstract class SingletonMonoBehaviour<T> : uLink.MonoBehaviour where T : SingletonMonoBehaviour<T> {
	private static T _instance = default(T);

	// Instance
	public static T instance {
		get {
			return _instance;
		}
	}

	// Awake
	protected virtual void Awake() {
		if(_instance == null) {
			_instance = (T)this;
			DontDestroyOnLoad(gameObject);
		} else if(_instance != this) {
			Destroy(gameObject);
		}
	}

	// Async
	public static Coroutine Async(IEnumerator enumerator) {
		return _instance.StartCoroutine(enumerator);
	}
}

// DestroyableSingletonMonoBehaviour
public abstract class DestroyableSingletonMonoBehaviour<T> : uLink.MonoBehaviour where T : DestroyableSingletonMonoBehaviour<T> {
	public static T instance = default(T);
	
	protected virtual void Awake() {
		instance = (T)this;
	}
}