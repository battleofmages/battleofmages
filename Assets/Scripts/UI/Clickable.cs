using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace BoM.UI {
	public class Clickable : MonoBehaviour, IPointerDownHandler {
		public UnityEvent onClick;
		
		public void OnPointerDown(PointerEventData eventData) {
			onClick.Invoke();
		}
	}
}
