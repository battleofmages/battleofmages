using uLobby;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Delegate: IPAndPortCallBack
public delegate void IPAndPortCallBack(string host, int port);

// NetworkHelper
public class NetworkHelper : SingletonMonoBehaviour<NetworkHelper> {
	public const int maxWWWTries = 5;
	public static Dictionary<string, Texture> urlToTexture = new Dictionary<string, Texture>();

	// Disable network emulation
	public static void DisableNetworkEmulation() {
		uLink.Network.emulation.minLatency = 0;
		uLink.Network.emulation.maxLatency = 0;
		uLink.Network.emulation.maxBandwidth = 0;
		uLink.Network.emulation.chanceOfDuplicates = 0;
		uLink.Network.emulation.chanceOfLoss = 0;
	}

	// URLRequest
	struct URLRequest {
		public string url;
		public int retries;
	}

	// GetTexture
	public delegate void TextureCallBack(Texture tex);
	public static void GetTexture(string url, TextureCallBack callBack) {
		// Cached?
		Texture texture;

		if(urlToTexture.TryGetValue(url, out texture)) {
			callBack(texture);
			return;
		}

		// Not cached: Download it!
		NetworkHelper.Async(GetTextureCoroutine(new URLRequest {
			url = url,
			retries = 0
		}, callBack));
	}

	// GetTextureCoroutine
	static IEnumerator GetTextureCoroutine(URLRequest urlRequest, TextureCallBack callBack) {
		var request = new WWW(urlRequest.url);
		yield return request;

		if(!string.IsNullOrEmpty(request.error)) {
			LogManager.General.LogError("Couldn't load texture: " + urlRequest.url + " (" + request.error + ")");

			// Re-download it
			urlRequest.retries += 1;

			if(urlRequest.retries < maxWWWTries) {
				NetworkHelper.Async(GetTextureCoroutine(urlRequest, callBack));
			}

			yield break;
		}

		var texture = request.texture;
		urlToTexture[urlRequest.url] = texture;
		callBack(texture);
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