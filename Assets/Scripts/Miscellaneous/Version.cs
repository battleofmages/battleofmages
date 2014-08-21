using UnityEngine.UI;

public class Version : SingletonMonoBehaviour<Version> {
	// Version number
	public int versionNumber;

	// Start
	void Start() {
		var versionText = GetComponent<Text>();
		versionText.text = versionText.text.Replace("{version}", GUIHelper.MakePrettyVersion(versionNumber));
	}
}
