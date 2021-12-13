using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Clickable : MonoBehaviour, IPointerDownHandler {
	public UnityEvent onClick;
	
	public void OnPointerDown(PointerEventData eventData) {
		onClick.Invoke();
	}
}