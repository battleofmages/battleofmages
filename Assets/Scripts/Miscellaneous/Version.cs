using UnityEngine;
using UnityEngine.UI;
using uLobby;

public class Version : SingletonMonoBehaviour<Version> {
	// Version number
	public int clientVersionNumber;
	public Text versionLabel;

	// Server version number
	public int serverVersionNumber {
		get;
		protected set;
	}

	// Start
	void Start() {
		// Receive lobby RPCs
		Lobby.AddListener(this);
	
		// Update version text
		versionLabel.text = versionLabel.text.Replace("{version}", GUIHelper.MakePrettyVersion(clientVersionNumber));
	}

	[RPC]
	void VersionNumber(int nServerVersionNumber) {
		serverVersionNumber = nServerVersionNumber;

		LogManager.General.Log("Client version: " + clientVersionNumber + ", server version: " + serverVersionNumber);
	}
}
