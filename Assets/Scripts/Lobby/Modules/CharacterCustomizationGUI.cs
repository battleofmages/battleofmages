using uLobby;
using UnityEngine;

public sealed class CharacterCustomizationGUI : LobbyModule<CharacterCustomizationGUI> {
	public GUIContent voicePlayButtonContent;
	
	private Vector2 scrollPosition;
	private CharacterCustomization custom;
	private float lastPlayTime;
	private MouseLook mouseLook;
	private Transform characterModel;
	
	// Start customization
	public void StartCustomization() {
		LogManager.General.Log("Starting character customization");
		
		scrollPosition = Vector2.zero;
		CharacterPreview.instance.gameObject.SetActive(true);
		mouseLook = CharacterPreview.instance.GetComponent<MouseLook>();
		
		var prefab = Config.instance.femalePrefab;
		if(prefab == null)
			LogManager.General.LogError("Character prefab not defined!");
		
		characterModel = ((GameObject)Object.Instantiate(prefab)).transform;
		characterModel.parent = CharacterPreview.instance.transform;
		characterModel.localPosition = Cache.vector3Zero;
		characterModel.localRotation = Cache.quaternionIdentity;
	}
	
	// End customization
	public void EndCustomization() {
		LogManager.General.Log("Ending character customization");
		
		if(characterModel != null) {
			Destroy(characterModel.gameObject);
			characterModel = null;
		}
		
		CharacterPreview.instance.gameObject.SetActive(false);
	}
	
	// Draw
	public override void Draw() {
		custom = InGameLobby.instance.displayedAccount.custom;
		if(custom == null)
			return;
		
		int padding = 4;
		int width = (int)GUIArea.width / 4;
		Vector3 minRGB = new Vector3(0.05f, 0.05f, 0.05f);
		
		GUILayout.BeginArea(new Rect(GUIArea.width - width + padding, padding, width - padding * 2, GUIArea.height - padding * 2));
		using(new GUIVertical("box")) {
			using(new GUIScrollView(ref scrollPosition)) {
				using(new GUIHorizontal()) {
					GUI.enabled = false;
					GUILayout.Button("Male", GUILayout.Width(70));
					GUILayout.Button("Female", GUILayout.Width(70));
					GUI.enabled = true;
					GUILayout.FlexibleSpace();
					GUILayout.Label("Coming soon!");
				}
				
				GUILayout.Label("Height:");
				custom.height = GUILayout.HorizontalSlider(custom.height, 0f, 1f);
				
				using(new GUIHorizontal()) {
					GUILayout.Label("Voice:");
					GUILayout.FlexibleSpace();
					if(GUILayout.Button(voicePlayButtonContent)) {
						if(GameManager.inGame) {
							if(Player.main != null) {
								Player.main.audio.clip = Magic.instance.randomCastVoiceClip;
								Player.main.audio.Play();
							}
						} else {
							CharacterPreview.instance.audio.clip = Magic.instance.randomCastVoiceClip;
							CharacterPreview.instance.audio.Play();
						}
					}
				}
				//using(new GUIHorizontal()) {
					//GUILayout.Label("Mature", GUILayout.Width(60f));
					//using(new GUIVertical()) {
						//GUILayout.Space(2);
						custom.voicePitch = GUILayout.HorizontalSlider(custom.voicePitch, 0f, 1f);
						if(GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
							GUI.tooltip = "Childish <-----> Mature";
					//}
					//GUILayout.Label("Childish", GUILayout.Width(60f));
				//}
				
				GUILayout.Label("Hair color:");
				custom.hairColor = GUIHelper.ColorPicker(custom.hairColor, minRGB);
				
				GUILayout.Label("Eye color:");
				custom.eyeColor = GUIHelper.ColorPicker(custom.eyeColor, new Vector3(0.3f, 0.3f, 0.3f));
				
				GUILayout.Label("Eye background color:");
				custom.eyeBackgroundColor = GUIHelper.ColorPicker(custom.eyeBackgroundColor, minRGB);
				
				GUILayout.Label("Cloak color:");
				custom.cloakColor = GUIHelper.ColorPicker(custom.cloakColor, minRGB);
				
				GUILayout.Label("Topwear color:");
				custom.topWearColor = GUIHelper.ColorPicker(custom.topWearColor, minRGB);
				
				GUILayout.Label("Legwear color:");
				custom.legWearColor = GUIHelper.ColorPicker(custom.legWearColor, minRGB);
				
				GUILayout.Label("Boots color:");
				custom.bootsColor = GUIHelper.ColorPicker(custom.bootsColor, minRGB);
			}
			
			GUILayout.FlexibleSpace();
			
			if(Player.main == null) {
				if(GUIHelper.Button("Finish")) {
					Sounds.instance.PlayButtonClick();
					SendCustomization();
				}
			}
		}
		GUILayout.EndArea();
		
		// Visualize changes
		UpdateCustomization();
	}
	
	// Send customization
	void SendCustomization() {
		// Lobby server
		Lobby.RPC("ClientCharacterCustomization", Lobby.lobby, custom);
		
		// Ingame, town server
		if(Player.main != null)
			Player.main.networkView.RPC("ClientCharacterCustomization", uLink.RPCMode.Server, custom);
	}
	
	// On leaving the NPC
	public override void OnNPCExit() {
		SendCustomization();
	}
	
	// Update customization
	public void UpdateCustomization() {
		if(Player.main != null) {
			Player.main.ReceiveCharacterCustomization(custom);
		} else if(CharacterPreview.instance != null && custom != null) {
			CharacterPreview.instance.transform.localScale = custom.scaleVector;
			
			AudioSource audioSource = CharacterPreview.instance.audio;
			float newPitch = custom.finalVoicePitch;
			if(audioSource.pitch != newPitch) {
				audioSource.pitch = newPitch;
				
				if(GUI.changed && !audioSource.isPlaying && Time.time - lastPlayTime > 1.5f) {
					if(audioSource.clip == null) {
						audioSource.volume = 0.4f;
						audioSource.clip = Magic.instance.randomCastVoiceClip;
					}
					
					audioSource.Play();
					
					lastPlayTime = Time.time;
				}
			}
			
			InGameLobby.instance.displayedAccount.custom.UpdateMaterials(characterModel);
			
			mouseLook.enabled = Input.GetMouseButton(1);
		}
	}
	
	// --------------------------------------------------------------------------------
	// RPCs
	// --------------------------------------------------------------------------------
}
