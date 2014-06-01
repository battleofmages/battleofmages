// (c)2011 MuchDifferent. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;
using uLink;

/// <summary>
/// Use this script to make a pool of pre-instantiated prefabs that will be used 
/// when prefabs are network instantiated via uLink.Network.Instantiate().
/// </summary>
///
/// <remarks>
/// This wonderful script makes it possible to pool game objects (instantiated prefabs)
/// and uLink will use this pool of objects as soon as the gameplay code makes a call to
/// Network.Instantiate.
///
/// The wonderful part is that you do not have to change your code at all to use the pool
/// mechanism. This makes it very easy to check if your game is faster and more smooth
/// with a pool.
///
/// In some games, with lots of spawning NPCs, guided missiles etc, the pool
/// is very convenient to avoid the overhead of calling Object.Instantiate() and Object.Destroy().
///
/// The need for a pool is mainly to increase performance, Instantiating objects is a 
/// heavy operation in Unity. By load-testing the game with the uTsung tool it is easier to
/// make decisions when to use a pool for prefabs.
///
/// A normal scenario is to pool creator prefabs in the server scene and pool proxy prefabs
/// in the client scene.
/// 
/// The value minSize is the number of prefabs that will be instantiated at startup.
/// If the number of Instantiate calls does go above this value, the pool will be increased 
/// at run-time.
/// </remarks>
[AddComponentMenu("uLink Utilities/Instantiate Pool")]
public class uLinkInstantiatePool : uLink.MonoBehaviour
{
	public uLink.NetworkView prefab;

	public int minSize = 50; // This number of prefabs will be instantiated at startup

	private readonly Stack<uLink.NetworkView> pool = new Stack<uLink.NetworkView>();

	private Transform parent;

	void Awake()
	{
		if (enabled) CreatePool();
	}

	void Start()
	{
		// this is here just so the componet can be enabled/disabled.
	}

	void OnDisable()
	{
		DestroyPool();
	}

	public void CreatePool()
	{
		if (prefab._manualViewID != 0)
		{
			Debug.LogError("Prefab viewID must be set to Allocated or Unassigned", prefab);
			return;
		}

		parent = new GameObject(name + "-Pool").transform;

		for (int i = 0; i < minSize; i++)
		{
			uLink.NetworkView instance = (uLink.NetworkView)Instantiate(prefab); // will trigger callback message "Awake" and "OnEnable" (networkView.viewID == unassigned).

			SetActive(instance, false); // will trigger callback message "OnDisable" (networkView.viewID == unassigned).

			instance.transform.parent = parent;

			pool.Push(instance);
		}

		uLink.NetworkInstantiator.Add(prefab.name, Creator, Destroyer);
	}

	public void DestroyPool()
	{
		uLink.NetworkInstantiator.Remove(prefab.name);
		pool.Clear();

		// This is being done due to a race condition. Switching rooms might result the parent object being destroyed earlier.
		if (parent != null)
		{
			Destroy(parent.gameObject);
			parent = null;
		}
	}

	private uLink.NetworkView Creator(string prefabName, uLink.NetworkInstantiateArgs args, uLink.NetworkMessageInfo info)
	{
		uLink.NetworkView instance;

		if (pool.Count > 0)
		{
			instance = pool.Pop();

			args.SetupNetworkView(instance);

			SetActive(instance, true); // will trigger callback message "OnEnable" (networkView.viewID != unassigned).
		}
		else
		{
			instance = uLink.NetworkInstantiatorUtility.Instantiate(prefab, args); // will trigger callback message "Awake" and "OnEnable" (networkView.viewID != unassigned).
		}

		uLink.NetworkInstantiatorUtility.BroadcastOnNetworkInstantiate(instance, info); // will trigger callback message "OnNetworkInstantiate".

		return instance;
	}

	private void Destroyer(uLink.NetworkView instance)
	{
		SetActive(instance, false); // will trigger callback message "OnDisable" (networkView.viewID != unassigned).

		pool.Push(instance);
	}

	private static void SetActive(uLink.NetworkView instance, bool value)
	{
#if UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_2 || UNITY_3_1 || UNITY_3_0 || UNITY_2_6
		instance.gameObject.SetActiveRecursively(value); // Unity 3.x or older
#else
		instance.gameObject.SetActive(value); // Unity 4.x or later
#endif
	}
}
