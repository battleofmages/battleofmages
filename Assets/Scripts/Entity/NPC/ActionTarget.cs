using UnityEngine;

public interface ActionTarget {
	void OnAction(Entity entity);
	bool CanAction(Entity entity);
	Vector3 GetCursorPosition();
}