#if !UNITY_WEBPLAYER
using System.IO;
#endif

using UnityEngine;

public class LogCategory {
	public static string logPath = "./logs/"; 
	public static string timeFormat = "yyyy-MM-dd HH:mm:ss.fff";
	
#if !UNITY_WEBPLAYER
	public string filePath;
	private StreamWriter writer;
#endif
	
	private bool useUnityDebugLog;
	
	// Static init
	public static void Init(string newLogPath) {
		logPath = newLogPath;
		
#if !UNITY_WEBPLAYER
		if(!Directory.Exists(logPath))
			Directory.CreateDirectory(logPath);
#endif
	}
	
	// Constructor
	public LogCategory(string categoryName, bool nUseUnityDebugLog = true, bool autoFlush = true) {
#if !UNITY_WEBPLAYER
		filePath = logPath + categoryName + ".log";
		writer = File.AppendText(filePath);
		writer.AutoFlush = autoFlush;
#endif
#if UNITY_EDITOR || UNITY_WEBPLAYER
		useUnityDebugLog = nUseUnityDebugLog;
#endif
	}
	
	// Log
	public void Log(object msg) {
#if !UNITY_WEBPLAYER
		writer.WriteLine(System.DateTime.UtcNow.ToString(timeFormat) + ": " + msg);
#endif
#if UNITY_EDITOR || UNITY_WEBPLAYER
		if(useUnityDebugLog)
			Debug.Log(string.Concat("<color=#808080>", System.DateTime.UtcNow.ToString(timeFormat), ":</color> <color=#dddddd>", msg.ToString().Replace("\n", "</color>\n<color=#dddddd>"), "</color>"));
#endif
	}
	
	// LogWarning
	public void LogWarning(object msg) {
#if !UNITY_WEBPLAYER
		writer.WriteLine(System.DateTime.UtcNow.ToString(timeFormat) + ": [WARNING] " + msg);
#endif
#if UNITY_EDITOR || UNITY_WEBPLAYER
		if(useUnityDebugLog)
			Debug.LogWarning(string.Concat("<color=#808080>", System.DateTime.UtcNow.ToString(timeFormat), ":</color> <color=#dddddd>", msg.ToString().Replace("\n", "</color>\n<color=#dddddd>"), "</color>"));
#endif
	}
	
	// LogError
	public void LogError(object msg) {
#if !UNITY_WEBPLAYER
		writer.WriteLine(System.DateTime.UtcNow.ToString(timeFormat) + ": [ERROR] " + msg);
#endif
#if UNITY_EDITOR || UNITY_WEBPLAYER
		if(useUnityDebugLog)
			Debug.LogError(string.Concat("<color=#808080>", System.DateTime.UtcNow.ToString(timeFormat), ":</color> <color=#dddddd>", msg.ToString().Replace("\n", "</color>\n<color=#dddddd>"), "</color>"));
#endif
	}
	
	// Close
	public void Close() {
#if !UNITY_WEBPLAYER
		writer.Close();
#endif
	}
	
	// GenerateReport
	public void GenerateReport() {
		Log("Platform: " + Application.platform);
		Log("Unity player version: " + Application.unityVersion);
		Log("Device ID: " + SystemInfo.deviceUniqueIdentifier);
		
		if(Application.genuineCheckAvailable)
			Log("Genuine: " + Application.genuine);
		
		// CPU
		Log("Processor count: " + SystemInfo.processorCount);
		
		// RAM
		Log("System memory: " + SystemInfo.systemMemorySize + " MB");
		
		// GPU
		Log("Graphics device: " + SystemInfo.graphicsDeviceName);
		Log("Graphics memory: " + SystemInfo.graphicsMemorySize + " MB");
		Log("Graphics device version: " + SystemInfo.graphicsDeviceVersion);
		Log("Graphics device vendor: " + SystemInfo.graphicsDeviceVendor);
		Log("Shadows: " + (SystemInfo.supportsShadows ? "Supported" : "Not supported"));
		Log("Image Effects: " + (SystemInfo.supportsImageEffects ? "Supported" : "Not supported"));
		Log("Render Textures: " + (SystemInfo.supportsRenderTextures ? "Supported" : "Not supported"));
		
		// Misc
		Log("Internet reachability: " + Application.internetReachability);
		Log("Language: " + Application.systemLanguage);
		Log("Data path: " + Application.dataPath);
	}
}