// Singleton
public abstract class Singleton<T> where T : Singleton<T> {
	public static T instance = default(T);
}

// SingletonMonoBehaviour
public abstract class SingletonMonoBehaviour<T> : uLink.MonoBehaviour where T : SingletonMonoBehaviour<T> {
	public static T instance = default(T);
	
	protected virtual void Awake() {
		if(instance == null) {
			instance = (T)this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}
}

// DestroyableSingletonMonoBehaviour
public abstract class DestroyableSingletonMonoBehaviour<T> : uLink.MonoBehaviour where T : DestroyableSingletonMonoBehaviour<T> {
	public static T instance = default(T);
	
	protected virtual void Awake() {
		instance = (T)this;
	}
}