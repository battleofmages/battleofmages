using UnityEngine;

public abstract partial class Entity : uLink.MonoBehaviour, PartyMember<Entity> {
	// Death
	public event CallBack onDeath;

	// Kill
	public delegate void KillHandler(Entity killer, Entity target, short skillId);
	public event KillHandler onKill;

	// Destroy
	public event CallBack onDestroy;

	// InitEvents
	void InitEvents() {
		onDeath += () => {
			// Interrupt cast on death
			InterruptCast();

			// Make victim visible but make the name label invisible
			isVisible = true;
			
			if(nameLabel != null)
				nameLabel.enabled = false;
		};

		// Kill text
		onKill += (killer, victim, skillId) => {
			if(killer == Player.main)
				Entity.SpawnText(killer, "+Kill", new Color(1.0f, 1.0f, 0.5f, 1.0f), Random.Range(-10, 10), 30);
		};

		// Last kills window
		onKill += (killer, victim, skillId) => {
			if(LastKills.instance != null)
				LastKills.instance.AddEntry(killer.name, Skill.idToSkill[skillId], victim.name);
		};

		// Chat message
		onKill += (killer, victim, skillId) => {
			if(LobbyChat.instance == null)
				return;
			
			if(killer == Player.main)
				LobbyChat.instance.AddEntry("You killed " + victim.name + ".");
			else if(victim == Player.main)
				LobbyChat.instance.AddEntry("You got killed by " + killer.name + ".");
			else
				LobbyChat.instance.AddEntry(killer.name + " killed " + victim.name + ".");
		};
	}

	// OnDestroy
	void OnDestroy() {
		if(onDestroy != null)
			onDestroy();
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