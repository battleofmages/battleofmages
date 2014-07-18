using UnityEngine;
using System.Collections;
using uLobby;

public sealed class BuildsGUI : LobbyModule<BuildsGUI> {
	public GUIContent[] content;
	public TraitsGUI traitsGUI;
	public ArtifactsGUI artifactsGUI;
	public SkillsGUI skillsGUI;
	
	private int selectedPage;
	private int nextPage;
	
	void Start() {
		selectedPage = 0;
	}
	
	public override void Draw() {
		// Availability
		var availFuncs = new GUIHelper.IsAvailableFunc[] {
			() => { return InGameLobby.instance.displayedAccount.charStats != null; },
			null,
			null
		};
		
		// Draw callbacks
		var callBacks = new CallBack[] {
			// Traits
			() => {
				traitsGUI.Draw();
			},
			
			// Artifacts
			() => {
				artifactsGUI.Draw();
			},
			
			// Skills
			() => {
				skillsGUI.Draw();
			}
		};
		
		// Toolbar
		using(new GUIHorizontalCenter()) {
			nextPage = GUIHelper.Toolbar(selectedPage, content, availFuncs, GUILayout.Width(96));
		}
		
		// Draw page
		callBacks[selectedPage]();
	}
	
	void LateUpdate() {
		selectedPage = nextPage;
	}
	
	// On coming close to the NPC
	public override void OnNPCEnter() {
		InGameLobby.instance.currentLobbyModule = this;
	}
}
