using UnityEngine;
using System.Collections;
using uLobby;

public sealed class SettingsGUI : LobbyModule<SettingsGUI> {
	public GUIContent[] contents;
	
	public bool vSync;
	
	public GraphicsEffect[] effects;
	
	public GUIStyle settingNameStyle;
	public GUIStyle settingValueStyle;
	
	private int selectedPage;
	private int nextPage;
	private InputManager inputManager;
	
	private Vector2 scrollPosition;
	private Vector2 inputScrollPosition;
	
	private InputControl captureControl;
	private bool captureAltKey;
	private KeyCode lastKeyCaptured;
	private bool enableCapture = true;
	private bool modified = false;
	private LobbyChat lobbyChat;
	private MusicManager music;
	
	// Audio speaker modes
	private AudioSpeakerMode[] audioSpeakerModes = {
		AudioSpeakerMode.Stereo,
		AudioSpeakerMode.Prologic,
		AudioSpeakerMode.Quad,
		AudioSpeakerMode.Surround,
		AudioSpeakerMode.Mode5point1,
		AudioSpeakerMode.Mode7point1,
	};
	
	// Audio speaker mode names
	private string[] audioSpeakerModeNames = {
		"Stereo",
		"Prologic",
		"Surround 4.0",
		"Surround 5.0",
		"Surround 5.1",
		"Surround 7.1",
	};
	
	// Start
	void Start() {
		// Receive RPCs from lobby
		Lobby.AddListener(this);
		
		lobbyChat = this.GetComponent<LobbyChat>();
		
		var audioGameObject = GameObject.Find("Audio");
		if(audioGameObject != null) {
			music = audioGameObject.GetComponent<MusicManager>();
		}
		
		inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
		
		// Load settings
		
		// Graphics - Quality
		QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Graphics_QualityLevel", QualitySettings.GetQualityLevel()));
		
		/*vSync = PlayerPrefs.GetInt("Graphics_VSync", vSync != 0) != 0;
		sunShafts = PlayerPrefs.GetInt("Graphics_SunShafts", sunShafts != 0) != 0;
		bloom = PlayerPrefs.GetInt("Graphics_Bloom", bloom != 0) != 0;
		toneMapping = PlayerPrefs.GetInt("Graphics_ToneMapping", toneMapping != 0) != 0;
		vignetting = PlayerPrefs.GetInt("Graphics_Vignetting", vignetting != 0) != 0;
		contrastEnhance = PlayerPrefs.GetInt("Graphics_ContrastEnhance", contrastEnhance != 0) != 0;
		contrastStretch = PlayerPrefs.GetInt("Graphics_ContrastStretch", contrastStretch != 0) != 0;
		depthOfField = PlayerPrefs.GetInt("Graphics_DepthOfField", depthOfField != 0) != 0;
		*/
		
		vSync = PlayerPrefs.GetInt("Graphics_VSync", vSync ? 1 : 0) != 0;
		
		foreach(var effect in effects) {
			effect.activated = PlayerPrefs.GetInt("Graphics_" + effect.prefsId, effect.activated ? 1 : 0) != 0;
		}
		
		// Audio - Speaker mode
		AudioSettings.speakerMode = audioSpeakerModes[PlayerPrefs.GetInt("Audio_SpeakerMode", 0)];
		
		// Audio - Master volume
		AudioListener.volume = PlayerPrefs.GetFloat("Audio_MasterVolume", AudioListener.volume);
		
		// Audio - Music volume
		if(music != null) {
			// NOTE: Music manager sets the volume itself
			//music.volume = PlayerPrefs.GetFloat("Audio_MusicVolume", music.volume);
			music.PlayCategory("Lobby");
		}
		
		// Input - Mouse sensitivity
		inputManager.mouseSensitivity = PlayerPrefs.GetFloat("Input_MouseSensitivity", inputManager.mouseSensitivity);
	}
	
	// Draw
	public override void Draw() {
		using(new GUIHorizontal()) {
			nextPage = GUIHelper.Toolbar(selectedPage, contents);
		}
		
		GUILayout.BeginHorizontal("box");
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		GUILayout.BeginHorizontal();
		
		switch(selectedPage) {
			case 0:
				DrawGraphicsSettings();
				break;
				
			case 1:
				DrawAudioSettings();
				break;
				
			case 2:
				DrawInputSettings();
				break;
				
			case 3:
				DrawGameSettings();
				break;
		}
		
		GUILayout.EndHorizontal();
		GUILayout.EndScrollView();
		GUILayout.EndHorizontal();
	}
	
	// On coming close to the NPC
	public override void OnNPCEnter() {
		InGameLobby.instance.currentLobbyModule = this;
	}
	
	// Graphics
	void DrawGraphicsSettings() {
		// Keys
		using(new GUIVertical()) {
			GUILayout.Label("Quality preset", settingNameStyle);
			GUILayout.Label("Effects", settingNameStyle);
			
			/*foreach(var effect in effects) {
				GUILayout.Label(effect.name, settingNameStyle);
			}*/
			
			/*GUILayout.Label("VSync", settingNameStyle);
			GUILayout.Label("Texture quality", settingNameStyle);
			GUILayout.Label("Anisotropic filtering", settingNameStyle);
			GUILayout.Label("Pixel light count", settingNameStyle);
			GUILayout.Label("LOD bias", settingNameStyle);
			GUILayout.Label("Maximum LOD level", settingNameStyle);
			GUILayout.Label("Shadow projection", settingNameStyle);
			GUILayout.Label("Shadow distance", settingNameStyle);
			GUILayout.Label("Particle raycast budget", settingNameStyle);*/
		}
		
		// Values
		using(new GUIVertical()) {
			using(new GUIHorizontal()) {
				for(int i = 0; i < QualitySettings.names.Length; i++) {
					if(QualitySettings.GetQualityLevel() == i)
						GUI.backgroundColor = GUIColor.MenuItemActive;
					else
						GUI.backgroundColor = GUIColor.MenuItemInactive;
					
					if(GUIHelper.Button(QualitySettings.names[i])) {
						Sounds.instance.PlayButtonClick();
						QualitySettings.SetQualityLevel(i);
						PlayerPrefs.SetInt("Graphics_QualityLevel", i);
					}
				}
				GUI.backgroundColor = GUIColor.MenuItemInactive;
			}
			
			foreach(var effect in effects) {
				effect.activated = DrawGraphicsToggle(effect.activated, effect.name, effect.prefsId);
			}
			
			vSync = DrawGraphicsToggle(vSync, "VSync", "VSync");
			
			/*GUILayout.Label(QualitySettings.vSyncCount == 0 ? "Disabled" : "Enabled", settingValueStyle);
			GUILayout.Label(QualitySettings.masterTextureLimit == 0 ? "Full" : "Half", settingValueStyle);
			GUILayout.Label(QualitySettings.anisotropicFiltering.ToString(), settingValueStyle);
			GUILayout.Label(QualitySettings.pixelLightCount.ToString(), settingValueStyle);
			GUILayout.Label(QualitySettings.lodBias.ToString(), settingValueStyle);
			GUILayout.Label(QualitySettings.maximumLODLevel.ToString(), settingValueStyle);
			GUILayout.Label(QualitySettings.shadowProjection.ToString(), settingValueStyle);
			GUILayout.Label(QualitySettings.shadowDistance.ToString(), settingValueStyle);
			GUILayout.Label(QualitySettings.particleRaycastBudget.ToString(), settingValueStyle);*/
		}
	}
	
	// Draw graphics settings toggle
	bool DrawGraphicsToggle(bool val, string name, string prefsId) {
		bool newVal;
		
		using(new GUIHorizontal(GUILayout.Width(150))) {
			using(new GUIVerticalCenter(GUILayout.Height(28))) {
				if(val)
					GUI.backgroundColor = GUIColor.MenuItemActive;
				else
					GUI.backgroundColor = GUIColor.MenuItemInactive;
				
				newVal = val;
				if(GUIHelper.Button(name)) {
					newVal = !val;
				}
			}
		}
		
		if(newVal != val) {
			PlayerPrefs.SetInt("Graphics_" + prefsId, newVal ? 1 : 0);
			UpdateGraphicsSettings();
		}
		
		return newVal;
	}
	
	// Audio
	void DrawAudioSettings() {
		// Keys
		using(new GUIVertical()) {
			GUILayout.Label("Master volume", settingNameStyle);
			
			if(music != null)
				GUILayout.Label("Music volume", settingNameStyle);
			
			GUILayout.Label("Speaker mode", settingNameStyle);
			GUILayout.Label("Microphone volume", settingNameStyle);
			GUILayout.Label("Sample rate", settingNameStyle);
		}
		
		// Values
		using(new GUIVertical()) {
			// Master volume
			float newMasterVolume = GUIHelper.HorizontalSliderVCenter(AudioListener.volume, 0, 1, GUILayout.Height(24));
			if(newMasterVolume != AudioListener.volume) {
				AudioListener.volume = newMasterVolume;
				PlayerPrefs.SetFloat("Audio_MasterVolume", newMasterVolume);
			}
			
			// Music volume
			if(music != null) {
				float newMusicVolume = GUIHelper.HorizontalSliderVCenter(music.volume, 0, 1, GUILayout.Height(24));
				if(newMusicVolume != music.volume) {
					music.volume = newMusicVolume;
					PlayerPrefs.SetFloat("Audio_MusicVolume", newMusicVolume);
				}
			}
			
			// Speaker mode
			using(new GUIHorizontal()) {
				for(int i = 0; i < audioSpeakerModes.Length; i++) {
					var mode = audioSpeakerModes[i];
					
					if(mode == AudioSettings.speakerMode)
						GUI.backgroundColor = GUIColor.MenuItemActive;
					else
						GUI.backgroundColor = GUIColor.MenuItemInactive;
					
					if(GUIHelper.Button(audioSpeakerModeNames[i])) {
						Sounds.instance.PlayButtonClick();
						AudioSettings.speakerMode = mode;
						PlayerPrefs.SetInt("Audio_SpeakerMode", i);
					}
				}
			}
			
			// Microphone volume
			float newMicVolume = GUIHelper.HorizontalSliderVCenter(VoIP.volumeMultiplier, 0f, 2f, GUILayout.Height(24));
			if(newMicVolume != VoIP.volumeMultiplier) {
				VoIP.volumeMultiplier = newMicVolume;
				PlayerPrefs.SetFloat("VoIP_VolumeMultiplier", newMicVolume);
			}
				
			//GUILayout.Space(4);
			
			GUILayout.Label(AudioSettings.outputSampleRate.ToString() + " Hz", settingValueStyle);
			
			GUI.backgroundColor = Color.white;
		}
	}
	
	// Update graphics settings
	public static void UpdateGraphicsSettings() {
		var client = GameObject.Find("Client");
		
		if(client != null) {
			client.GetComponent<ClientInit>().UpdateGraphicsSettings();
		}
	}
	
	// Input
	void DrawInputSettings() {
		//var areaRect = GUILayoutUtility.GetLastRect();
		//var width = areaRect.width;
		int width = 300;
		
		GUILayout.BeginVertical();
		inputScrollPosition = GUILayout.BeginScrollView(inputScrollPosition);
		
		// Mouse
		GUILayout.BeginHorizontal();
		GUILayout.Label("Mouse sensitivity", settingNameStyle, GUILayout.Width(width));
		inputManager.mouseSensitivity = GUILayout.HorizontalSlider(inputManager.mouseSensitivity, 1.0f, 9.0f);
		GUILayout.EndHorizontal();
		
		// Keybinds
		for(int i = 0; i < inputManager.controls.Length; i++) {
			var control = inputManager.controls[i];
			
			if(!control.active)
				continue;
			
			GUILayout.BeginHorizontal();
			
			// Name
			GUI.contentColor = Color.white;
			GUI.enabled = (captureControl == null || captureControl == control);
			GUILayout.Label(control.name, settingNameStyle, GUILayout.Width(width));
			
			// Main key
			//GUI.enabled = (captureControl == null || (captureControl == control && captureAltKey == false));
			if(control == captureControl && !captureAltKey)
				GUI.contentColor = Color.red;
			else
				GUI.contentColor = Color.white;
			
			if(GUIHelper.Button(control.keyCodeString, settingValueStyle, GUILayout.Width(width)) && captureControl == null && enableCapture) {
				Sounds.instance.PlayButtonClick();
				captureControl = control;
				captureAltKey = false;
				lobbyChat.chatInputEnabled = false;
			}
			
			// Alt key
			//GUI.enabled = (captureControl == null || (captureControl == control && captureAltKey == true));
			if(control == captureControl && captureAltKey)
				GUI.contentColor = Color.red;
			else
				GUI.contentColor = Color.white;
			
			if(GUIHelper.Button(control.altKeyCodeString, settingValueStyle, GUILayout.Width(width)) && captureControl == null && enableCapture) {
				Sounds.instance.PlayButtonClick();
				captureControl = control;
				captureAltKey = true;
				lobbyChat.chatInputEnabled = false;
			}
			
			GUILayout.EndHorizontal();
		}
		
		// Capture a key
		if(captureControl != null) {
			// Cancel capturing with escape
			if(Event.current.keyCode == KeyCode.Escape) {
				captureControl = null;
				lobbyChat.chatInputEnabled = true;
			// Erase keycode with backspace
			} else if(Event.current.keyCode == KeyCode.Backspace) {
				captureControl.Erase(captureAltKey);
				captureControl = null;
				lobbyChat.chatInputEnabled = true;
			// Overwrite key code with currently pressed key
			} else {
				lastKeyCaptured = captureControl.Capture(captureAltKey);
				if(lastKeyCaptured != KeyCode.None) {
					captureControl = null;
					enableCapture = false;
					modified = true;
					
					// To be safe
					Invoke("EnableCapture", 1.0f);
				}
			}
			
			if(Event.current.isKey)
				Event.current.Use();
		}
		
		GUI.enabled = true;
		GUI.contentColor = Color.white;
		GUILayout.EndScrollView();
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUI.enabled = modified;
		
		if(modified)
			GUI.backgroundColor = Color.yellow;
		else
			GUI.backgroundColor = Color.white;
		
		if(GUIHelper.Button("Save", GUILayout.Width(96))) {
			Sounds.instance.PlayButtonClick();
			Lobby.RPC("ClientInputSettings", Lobby.lobby, Jboy.Json.WriteObject(new InputSettings(inputManager)));
			modified = false;
		}
		GUI.enabled = true;
		GUI.backgroundColor = Color.white;
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();
	}
	
	// Enable capture
	void EnableCapture() {
		if(captureControl == null) {
			enableCapture = true;
		}
	}
	
	// Block input until the mouse button up event has been triggered
	void LateUpdate() {
		selectedPage = nextPage;
		
		if(!enableCapture) {
			if(Input.GetKeyUp(lastKeyCaptured)) {
				enableCapture = true;
				lobbyChat.chatInputEnabled = true;
			}
		}
	}
	
	// Game
	void DrawGameSettings() {
		// Keys
		using(new GUIVertical()) {
			GUILayout.Label("Password:", settingNameStyle);
			GUILayout.Label("Skill bar: Margin bottom", settingNameStyle);
			GUILayout.Label("Skill icons: Size", settingNameStyle);
			GUILayout.Label("Skill icons: Margin left and right", settingNameStyle);
			GUILayout.Label("Skill icons: Margin bottom", settingNameStyle);
			GUILayout.Label("Mini skill icons: Size", settingNameStyle);
			
			GUILayout.Label("Advanced settings:", settingNameStyle);
			GUILayout.Label("Interpolation speed:", settingNameStyle);
			
			/*GUILayout.Label("Unity player version", settingNameStyle);
			GUILayout.Label("Graphics device", settingNameStyle);
			GUILayout.Label("Graphics memory", settingNameStyle);
			GUILayout.Label("Graphics device version", settingNameStyle);
			GUILayout.Label("Graphics device vendor", settingNameStyle);
			GUILayout.Label("Shadows", settingNameStyle);
			GUILayout.Label("Image Effects", settingNameStyle);
			GUILayout.Label("Render Textures", settingNameStyle);
			GUILayout.Label("Processor count", settingNameStyle);
			GUILayout.Label("System memory", settingNameStyle);*/
		}
		
		// Values
		using(new GUIVertical()) {
			// Password change
			if(GUIHelper.Button("Change password")) {
				new PasswordChangeWindow(
					"Change your password",
					(oldPW, newPW) => {
						Lobby.RPC("ChangePassword", Lobby.lobby, GameDB.EncryptPasswordString(newPW));
					},
					null
				);
			}
			
			// Skill bar: Margin bottom
			int newSkillMiniIconMarginBottom = Mathf.RoundToInt(GUIHelper.HorizontalSliderVCenter(
				SkillBar.miniMarginBottom,
				0,
				12,
				GUILayout.Height(24)
			));
			if(newSkillMiniIconMarginBottom != SkillBar.miniMarginBottom) {
				SkillBar.miniMarginBottom = newSkillMiniIconMarginBottom;
				PlayerPrefs.SetInt("Game_SkillBar_MarginBottom", newSkillMiniIconMarginBottom);
			}
			
			// Icon size
			int newSkillIconSize = Mathf.RoundToInt(GUIHelper.HorizontalSliderVCenter(
				SkillBar.mainIconSize,
				SkillBar.defaultMainIconSize - 16,
				SkillBar.defaultMainIconSize + 16,
				GUILayout.Height(24)
			));
			if(newSkillIconSize != SkillBar.mainIconSize) {
				SkillBar.mainIconSize = newSkillIconSize;
				PlayerPrefs.SetInt("Game_SkillBar_IconSize", newSkillIconSize);
			}
			
			// Icon margin
			int newSkillIconMargin = Mathf.RoundToInt(GUIHelper.HorizontalSliderVCenter(
				SkillBar.mainMargin,
				SkillBar.defaultMainMargin - 6,
				SkillBar.defaultMainMargin + 6,
				GUILayout.Height(24)
			));
			if(newSkillIconMargin != SkillBar.mainMargin) {
				SkillBar.mainMargin = newSkillIconMargin;
				PlayerPrefs.SetInt("Game_SkillBar_IconMargin", newSkillIconMargin);
			}
			
			// Icon margin bottom
			int newSkillIconMarginBottom = Mathf.RoundToInt(GUIHelper.HorizontalSliderVCenter(
				SkillBar.mainMarginBottom,
				SkillBar.defaultMainMarginBottom - 10,
				SkillBar.defaultMainMarginBottom + 10,
				GUILayout.Height(24)
			));
			if(newSkillIconMarginBottom != SkillBar.mainIconSize) {
				SkillBar.mainMarginBottom = newSkillIconMarginBottom;
				PlayerPrefs.SetInt("Game_SkillBar_IconMarginBottom", newSkillIconMarginBottom);
			}
			
			// Mini icon size
			int newSkillMiniIconSize = Mathf.RoundToInt(GUIHelper.HorizontalSliderVCenter(
				SkillBar.miniIconSize,
				SkillBar.defaultMiniIconSize - 8,
				SkillBar.defaultMiniIconSize + 8,
				GUILayout.Height(24)
			));
			if(newSkillMiniIconSize != SkillBar.miniIconSize) {
				SkillBar.miniIconSize = newSkillMiniIconSize;
				PlayerPrefs.SetInt("Game_SkillBar_MiniIconSize", newSkillMiniIconSize);
			}
			
			GUILayout.Space(24);
			
			// Interpolation speed
			Config.instance.proxyInterpolationSpeed = GUIHelper.HorizontalSliderVCenter(
				Config.instance.proxyInterpolationSpeed,
				10,
				20,
				GUILayout.Height(24)
			);
			
			/*GUILayout.Label(Application.unityVersion, settingValueStyle);
			GUILayout.Label(SystemInfo.graphicsDeviceName, settingValueStyle);
			GUILayout.Label(SystemInfo.graphicsMemorySize + " MB", settingValueStyle);
			GUILayout.Label(SystemInfo.graphicsDeviceVersion, settingValueStyle);
			GUILayout.Label(SystemInfo.graphicsDeviceVendor, settingValueStyle);
			GUILayout.Label(SystemInfo.supportsShadows ? "Supported" : "Not supported", settingValueStyle);
			GUILayout.Label(SystemInfo.supportsImageEffects ? "Supported" : "Not supported", settingValueStyle);
			GUILayout.Label(SystemInfo.supportsRenderTextures ? "Supported" : "Not supported", settingValueStyle);
			GUILayout.Label(SystemInfo.processorCount.ToString(), settingValueStyle);
			GUILayout.Label(SystemInfo.systemMemorySize.ToString() + " MB", settingValueStyle);*/
		}
	}
	
#region RPCs
	// --------------------------------------------------------------------------------
	// RPCs
	// --------------------------------------------------------------------------------
	
	[RPC]
	void ReceiveInputSettings(string inputSettingsString) {
		InputSettings inputSettings = Jboy.Json.ReadObject<InputSettings>(inputSettingsString);
		inputManager.CopySettingsFrom(inputSettings);
	}
	
	[RPC]
	void ReceiveInputSettingsError() {
		LogManager.General.Log("Player doesn't have any saved input settings yet");
	}
	
	[RPC]
	void PasswordChangeSuccess() {
		LogManager.General.Log("Successfully changed password");
	}
	
	[RPC]
	void PasswordChangeError(string error) {
		LogManager.General.LogError("Error changing password: " + error);
	}
#endregion
}
