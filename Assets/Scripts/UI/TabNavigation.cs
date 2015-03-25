//Author: Melang http://forum.unity3d.com/members/melang.593409/
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
			if(UnityEngine.Input.GetKeyDown(KeyCode.Tab) && system.currentSelectedGameObject) {
				Selectable current = system.currentSelectedGameObject.GetComponent<Selectable>();

				if(!current)
					return;

				Selectable next = current.FindSelectableOnDown();

				if(next!= null) {
					InputField inputField = next.GetComponent<InputField>();

					if(inputField !=null)
						inputField.OnPointerClick(new PointerEventData(system));  //if it's an input field, also set the text caret

					system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
				}
				//else Debug.Log("next nagivation element not found");
			}
		}
	}
}