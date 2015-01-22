using uLobby;
using UnityEngine;
using System.Collections;

// Delegate: IPAndPortCallBack
public delegate void IPAndPortCallBack(string host, int port);

// NetworkHelper
public class NetworkHelper : SingletonMonoBehaviour<NetworkHelper> {
	// Disable network emulation
	public static void DisableNetworkEmulation() {
		uLink.Network.emulation.minLatency = 0;
		uLink.Network.emulation.maxLatency = 0;
		uLink.Network.emulation.maxBandwidth = 0;
		uLink.Network.emulation.chanceOfDuplicates = 0;
		uLink.Network.emulation.chanceOfLoss = 0;
	}

	// GetTexture
	public delegate void TextureCallBack(Texture tex);
	public static void GetTexture(string url, TextureCallBack callBack) {
		NetworkHelper.instance.StartCoroutine(GetTextureCoroutine(url, callBack));
	}

	// GetTextureCoroutine
	static IEnumerator GetTextureCoroutine(string url, TextureCallBack callBack) {
		var request = new WWW(url);
		yield return request;
		if(!string.IsNullOrEmpty(request.error))
			LogManager.General.LogError("Couldn't load texture: " + url);
		callBack(request.texture);
	}
	
	// Init public lobby key
	public static void InitPublicLobbyKey() {
		// Public key
		Lobby.publicKey = new PublicKey(
			"td076m4fBadO7bRuEkoOaeaQT+TTqMVEWOEXbUBRXZwf1uR0KE8A/BbOWNripW1eZinvsC+skgVT/G8mrhYTWVl0TrUuyOV6rpmgl5PnoeLneQDEfrGwFUR4k4ijDcSlNpUnfL3bBbUaI5XjPtXD+2Za2dRXT3GDMrePM/QO8xE=",
			"EQ=="
		);
	}
	
	// Download IP and port
	public static IEnumerator DownloadIPAndPort(string url, IPAndPortCallBack func) {
		if(func == null)
			yield break;
		
		var ipRequest = new WWW(url);
		yield return ipRequest;
		
		if(ipRequest.error == null) {
			string ipAndPort = ipRequest.text;
			string[] parts = ipAndPort.Split(':');
			
			var host = parts[0];
			var port = int.Parse(parts[1]);
			
			func(host, port);
		}
	}
}