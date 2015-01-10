using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TypingSound : MonoBehaviour {
	public AudioClip typingSound;
	private string oldText = "";

	// Play
	public void Play() {
		typingSound.Play();
	}

	// Update
	void Update() {
		var selectedObject = EventSystem.current.currentSelectedGameObject;

		if(!selectedObject) {
			oldText = null;
			return;
		}

		var inputField = selectedObject.GetComponent<InputField>();

		if(!inputField) {
			oldText = null;
			return;
		}

		if(inputField.text != oldText && Input.anyKey) {
			if(oldText != null)
				Play();

			oldText = inputField.text;
		}
	}
}
