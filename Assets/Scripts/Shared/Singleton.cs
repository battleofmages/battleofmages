/*
 * ==============================================
 * http://en.wikipedia.org/wiki/Singleton_pattern
 * ==============================================
 * "There is criticism of the use of the singleton pattern,
 * as some consider it an anti-pattern, judging that it is overused,
 * introduces unnecessary restrictions in situations where a sole instance
 * of a class is not actually required, and introduces global state
 * into an application."
 * 
 * HOWEVER in Unity we need an actual instance because of MonoBehaviour methods and coroutines.
 */

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
