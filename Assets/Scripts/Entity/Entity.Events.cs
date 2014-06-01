public abstract partial class Entity : uLink.MonoBehaviour, PartyMember<Entity> {
	// Death
	public delegate void DeathHandler();
	public event DeathHandler onDeath;

	// Kill
	public delegate void KillHandler(Entity target);
	public event KillHandler onKill;

	// InitEvents
	void InitEvents() {
		onDeath += OnDeath;
	}

	// OnDestroy
	protected virtual void OnDestroy() {
		// ...
	}
	
	// OnDeath
	void OnDeath() {
		InterruptCast();
	}
	
	// OnGameStarted
	void OnGameStarted() {
		InterruptCast();
	}
	
	// OnGameEnded
	void OnGameEnded() {
		InterruptCast();
	}
}