using UnityEngine;
using System.Collections.Generic;

public class WWWResource<T> {
	public static Dictionary<string, T> keyToData = new Dictionary<string, T>();

	public string key;
	public string url;
	public WWW request;
	protected T cached;
	protected int retries;
	
	// Constructor
	public WWWResource(string nKey, string nURL) {
		key = nKey;
		url = nURL.Replace(" ", "%20");

		if(!WWWResource<T>.keyToData.TryGetValue(key, out cached))
			StartRequest();
	}
	
	// Data
	public T data {
		get {
			if(GameManager.isClient && cached == null) {
				if(request == null) {
					StartRequest();
				} else if(request.isDone) {
					if(!string.IsNullOrEmpty(request.error)) {
						LogManager.General.LogError(string.Format("[{0}] {1}", request.error, url));

						request = null;
						StartRequest();

						return default(T);
					} else if(typeof(T) == typeof(Texture2D)) {
						cached = (T)(object)request.texture;
					} else if(typeof(T) == typeof(AudioClip)) {
						cached = (T)(object)request.GetAudioClip(false);
					} else if(typeof(T) == typeof(string)) {
						cached = (T)(object)request.text;
					}

					keyToData[key] = cached;
				}
			}
			
			return cached;
		}
	}
	
	// StartRequest
	void StartRequest() {
		if(retries >= 10)
			return;

		request = new WWW(url);
		retries += 1;
	}
}
