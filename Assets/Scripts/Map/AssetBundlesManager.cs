using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetBundlesManager : SingletonMonoBehaviour<AssetBundlesManager> {
	// URL for version info
	public string assetBundlesVersionURL;

	// URL for asset bundles
	protected string assetBundlesURL;

	// Dictionary to provide the version number for a map
	protected Dictionary<string, int> mapNameToVersion = new Dictionary<string, int>();

	// The coroutine
	public bool isReady {
		get;
		protected set;
	}

	protected int retries;

	// Start
	void Start() {
		StartCoroutine(DownloadAssetBundleVersionInfo());
	}

	// Download asset bundle version info
	IEnumerator DownloadAssetBundleVersionInfo() {
		var bundlesInfo = new WWW(assetBundlesVersionURL);
		yield return bundlesInfo;
		
		if(bundlesInfo.error == null) {
			LogManager.General.Log("Successfully downloaded asset bundle version info");

			var lines = bundlesInfo.text.Split('\n');
			string line;

			foreach(var rawLine in lines) {
				line = rawLine.Trim();

				// Skip empty lines
				if(line.Length == 0)
					continue;

				var data = line.Split('=');
				var key = data[0].TrimEnd();
				var value = data[1].TrimStart();

				if(key == "URL") {
					assetBundlesURL = value.Replace("{os}", "windows");
					LogManager.General.Log("Asset bundles URL: " + assetBundlesURL);
				} else {
					var mapVersion = System.Convert.ToInt32(value);
					mapNameToVersion[key] = mapVersion;
					LogManager.Spam.Log("Map version of " + key + ": " + mapVersion);
				}
			}

			isReady = true;
		} else {
			LogManager.General.LogError("Failed downloading asset bundle version info: " + bundlesInfo.error);

			// Retry
			retries += 1;
			if(retries < 10)
				StartCoroutine(DownloadAssetBundleVersionInfo());
		}
	}

	// Get map URL
	public string GetMapURL(string mapName) {
		return assetBundlesURL.Replace("{map}", mapName.Replace(" ", "%20"));
	}

	// Get map version
	public int GetMapVersion(string mapName) {
		return mapNameToVersion[mapName];
	}
}
