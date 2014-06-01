// (c)2012 MuchDifferent. All Rights Reserved.

using UnityEngine;
using uLink;

[AddComponentMenu("uLink Utilities/Override Network Destroy")]
public class uLinkOverrideNetworkDestroy : uLink.MonoBehaviour
{
	public string broadcastMessage = "uLink_OnNetworkDestroy";
	public bool autoDestroyAfterMessage = true;

	private uLink.NetworkView mainNetworkView;
	private uLink.NetworkInstantiator.Destroyer oldDestroyer;

	protected void uLink_OnNetworkInstantiate(uLink.NetworkMessageInfo info)
	{
		mainNetworkView = info.networkView;

		// override the instance's NetworkInsantiator Destroyer delegate.
		oldDestroyer = mainNetworkView.instantiator.destroyer;
		mainNetworkView.instantiator.destroyer = OverrideDestroyer;
	}

	private void OverrideDestroyer(uLink.NetworkView instance)
	{
		if (autoDestroyAfterMessage)
		{
			instance.BroadcastMessage(broadcastMessage, SendMessageOptions.DontRequireReceiver);
			Destroy();
		}
		else
		{
			// if we're relying on the message receiver for cleanup, then make sure there is one.
			instance.BroadcastMessage(broadcastMessage, SendMessageOptions.RequireReceiver);
		}
	}

	public void Destroy()
	{
		if (oldDestroyer != null) oldDestroyer(mainNetworkView);
	}
}
