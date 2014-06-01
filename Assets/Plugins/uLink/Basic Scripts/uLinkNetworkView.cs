using UnityEngine;

/// <summary>
/// Attach this central uLink script component to any game object that needs to send and/or receive RPCs.
/// Also use this script component to be able to send and/or receive state synchronization.
/// </summary>
/// <remarks>
/// All network aware object need this central and important script component.
/// The most common way to use this component is this:
/// 1. Create a prefab and put it in the Resources folder (NOTE: This location is important. It is not OK with a subfolder).
/// 2. Add this script component to the prefab.
/// 3. Write a script that instantiates this prefab using one of the av avilable uLink.Network.Instantiate() methods.
/// 4. Done.
///
/// Another common way is to make the object network aware from the beginning as part of a scene:
/// 1. Add your game object to the hiearchy view for the scene.
/// 2. Attach this script component to the prefab.
/// 3. Assign a unique "manual view ID" to the script component. Use numbers starting from 1, 2, 3 and so on.
/// 4. Done. 
/// There is no need to call Instatiate() for this game object.
///
/// Read more about network views in the Network Views chapter in the uLink manual.
/// </remarks>
[AddComponentMenu("uLink Basics/Network View")]
public sealed class uLinkNetworkView : uLink.NetworkView
{
}
