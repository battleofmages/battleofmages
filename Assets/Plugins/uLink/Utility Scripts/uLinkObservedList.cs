// (c)2011 MuchDifferent. All Rights Reserved.

using UnityEngine;
using uLink;

/// <summary>
/// Implementation for the script component uLink.NetworkObservedList.
/// </summary>
/// <remarks>
/// This script component is used to set up a list of components to serialize for 
/// a game object or prefab with a uLink.NetworkView. 
/// 
/// One example of a game object with two components to serialize is a tank with a turret. The tank is
/// the game object, the turret is one component, the body of the tank is another component. When the state
/// of the tank is serialized the body includes position + velocity + rotation. The turret has another 
/// rotation. The send the comple tank state over the network using uLink state synchronization it is 
/// manadatory to add a uLink.NetworkView (script component) to the game object. Then add a 
/// uLink.NetworkObservedList (script component) to the game object. 
/// Add the tank body and the tank turret to the list in the uLink.NetworkObservedList component.
/// Make sure the observed property of the uLink.NetworkView component is the the uLink.NetworkObservedList component.
///
/// Another example is a game object with an animation script component. If the game designer wants to send animation 
/// state and position state, the uLink.NetworkObservedList can be used. THis is how to do that: 
/// Add a uLink.NetworkObservedList to the game object. Add the postions (your script) and the animation (your script) 
/// to the list. Your two scripts must implement the callback uLink_OnSerializeNetworkView. Make sure the observed 
/// property of the uLink.NetworkView component is the the uLink.NetworkObservedList component.
/// </remarks>

[AddComponentMenu("uLink Utilities/Observed List")]
public class uLinkObservedList : uLink.MonoBehaviour
{
	public Component[] observedList;

	private uLink.NetworkObserved[] _list;

	protected void uLink_OnSerializeNetworkView(uLink.BitStream stream, uLink.NetworkMessageInfo info)
	{
		if (observedList == null) return;

		_CheckList();

		foreach (uLink.NetworkObserved observed in _list)
		{
			if (observed.serializeProxy != null)
			{
				observed.serializeProxy(stream, info);
			}
			else
			{
				//Debug.LogError(uLink.NetworkObserved.EVENT_SERIALIZE_PROXY + " is missing in " + observed.component.GetType());
			}
		}
	}

	protected void uLink_OnSerializeNetworkViewOwner(uLink.BitStream stream, uLink.NetworkMessageInfo info)
	{
		if (observedList == null) return;

		_CheckList();

		foreach (uLink.NetworkObserved observed in _list)
		{
			if (observed.serializeOwner != null)
			{
				observed.serializeOwner(stream, info);
			}
			else
			{
				//Debug.LogError(uLink.NetworkObserved.EVENT_SERIALIZE_OWNER + " is missing in " + observed.component.GetType());
			}
		}
	}

	protected void uLink_OnHandoverNetworkView(uLink.BitStream stream, uLink.NetworkMessageInfo info)
	{
		if (observedList == null) return;

		_CheckList();

		foreach (uLink.NetworkObserved observed in _list)
		{
			if (observed.serializeHandover != null)
			{
				observed.serializeHandover(stream, info);
			}
			else
			{
				//Debug.LogError(uLink.NetworkObserved.EVENT_SERIALIZE_HANDOVER + " is missing in " + observed.component.GetType());
			}
		}
	}

	protected void uLink_OnSerializeNetworkViewCellProxy(uLink.BitStream stream, uLink.NetworkMessageInfo info)
	{
		if (observedList == null) return;

		_CheckList();

		foreach (uLink.NetworkObserved observed in _list)
		{
			if (observed.serializeCellProxy != null)
			{
				observed.serializeCellProxy(stream, info);
			}
			else
			{
				//Debug.LogError(uLink.NetworkObserved.EVENT_SERIALIZE_CELLPROXY + " is missing in " + observed.component.GetType());
			}
		}
	}

	private void _CheckList()
	{
		if (_list == null || _list.Length != observedList.Length)
		{
			_list = new uLink.NetworkObserved[observedList.Length];

			for (int i = 0; i < observedList.Length; i++)
			{
				_list[i] = new uLink.NetworkObserved(observedList[i]);
			}
		}
		else
		{
			for (int i = 0; i < observedList.Length; i++)
			{
				_list[i].UpdateBinding(observedList[i]);
			}
		}
	}
}
