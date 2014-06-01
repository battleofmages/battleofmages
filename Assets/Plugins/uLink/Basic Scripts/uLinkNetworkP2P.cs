using UnityEngine;

/// <summary>
/// Attach this script component to a game object that needs to be able to receive P2P connection attempts.
/// </summary>
/// <remarks>
/// When this script component is attached to a game object a UDP socket will be opened at runtime and this socket will
/// handle incoming connection attempts from other peers.
/// </remarks>
/// <seealso cref="uLinkP2PConnector"/>
[AddComponentMenu("uLink Basics/Network P2P")]
public sealed class uLinkNetworkP2P : uLink.NetworkP2P
{
}
