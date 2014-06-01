public interface ActionTarget {
	void OnAction(Entity entity);
	bool CanAction(Entity entity);
}