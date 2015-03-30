// Original author: Melang http://forum.unity3d.com/members/melang.593409/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BoM.UI {
	// TabNavigation
	public class TabNavigation : MonoBehaviour {
		private EventSystem system;

		// Start
		void Start() {
			system = EventSystem.current;
		}

		// Update
		void Update() {
			// Are we pressing the tab key?
			if(!UnityEngine.Input.GetKeyDown(KeyCode.Tab))
				return;

			// Do we have a selected game object?
			if(!system.currentSelectedGameObject)
				return;

			// Get the Selectable
			var current = system.currentSelectedGameObject.GetComponent<Selectable>();

			// Does the game object have a Selectable?
			if(!current)
				return;

			// Get the next focusable element
			var next = current.FindSelectableOnDown();

			// Is there something we can tab to?
			if(next == null)
				return;

			// Is it an input field?
			var inputField = next.GetComponent<InputField>();

			// If it's an input field, also set the text caret
			if(inputField != null)
				inputField.OnPointerClick(new PointerEventData(system));

			// Select it
			system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
		}
	}
}