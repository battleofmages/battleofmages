public abstract partial class Entity : uLink.MonoBehaviour, PartyMember<Entity> {
	// Cache
	protected bool networkViewIsMine;
	protected bool networkViewIsProxy;

	// InitNetworkView
	void InitNetworkView() {
		if(networkView) {
			networkViewIsMine = networkView.isMine;
			networkViewIsProxy = networkView.isProxy;
		}
	}
}