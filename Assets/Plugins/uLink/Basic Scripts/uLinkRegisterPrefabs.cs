// (c)2011 MuchDifferent. All Rights Reserved.

using UnityEngine;
using uLink;

/// <summary>
/// Use this script component to register the prefabs in the scene that will be
/// used in uLink.Network.Instantiate(). 
///
/// If all your prefabs are placed in the Resources folder, there is no need to use this
/// utility script. 
///
/// If you want to register prefabs at run-time, for example after downloading an asset 
/// bundle in a client, check out the uLink class NetworkInstantiator in the API doc.
///
/// Read more about registering prefabs in the manual chapter about Instantitating Objects.
/// </summary>
[AddComponentMenu("uLink Basics/Register Prefabs")]
public class uLinkRegisterPrefabs : uLink.RegisterPrefabs
{
}
