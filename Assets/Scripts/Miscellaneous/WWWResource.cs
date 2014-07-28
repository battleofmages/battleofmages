using UnityEngine;

public class WWWResource<T> {
	public string url;
	public WWW request;
	protected T cached;
	protected int retries;
	
	// Constructor
	public WWWResource(string nURL) {
		url = nURL.Replace(" ", "%20");
		StartRequest();
	}
	
	// Data
	public T data {
		get {
			if(!uLink.Network.isServer && cached == null) {
				if(request == null) {
					StartRequest();
				} else if(request.isDone) {
					if(!string.IsNullOrEmpty(request.error)) {
						LogManager.General.LogError(string.Format("[{0}] {1}", request.error, url));

						request = null;
						StartRequest();
					} else if(typeof(T) == typeof(Texture2D)) {
						cached = (T)(object)request.texture;
					} else if(typeof(T) == typeof(AudioClip)) {
						cached = (T)(object)request.GetAudioClip(false);
					} else if(typeof(T) == typeof(string)) {
						cached = (T)(object)request.text;
					}
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
