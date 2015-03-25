using UnityEngine;
using UnityEngine.UI;

public class OnEnableReset : MonoBehaviour {
	// OnEnable
	void OnEnable() {
		GetComponent<InputField>().text = "";
	}
}
