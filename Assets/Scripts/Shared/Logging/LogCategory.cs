using System.IO;

using UnityEngine;

public class LogCategory {
	public static string logPath = "./logs/"; 
	public static string timeFormat = "yyyy-MM-dd HH:mm:ss.fff";
	public static string timeColor;
	public static string messageColor;

	public string filePath;
	private StreamWriter writer;
	
	private bool useUnityDebugLog;
	
	// Static init
	public static void Init(string newLogPath) {
		logPath = newLogPath;
		
		if(!Directory.Exists(logPath))
			Directory.CreateDirectory(logPath);
		
#if UNITY_PRO_LICENSE
		timeColor = "#808080";
		messageColor = "#dddddd";
#else
		timeColor = "#00156B";
		messageColor = "#0033FF";
#endif
	}
	
	// Constructor
	public LogCategory(string categoryName, bool nUseUnityDebugLog = true, bool autoFlush = true) {
		LogManager.list.Add(this);
		filePath = logPath + categoryName + ".log";
		writer = File.AppendText(filePath);
		writer.AutoFlush = autoFlush;
#if UNITY_EDITOR
		useUnityDebugLog = nUseUnityDebugLog;
#endif
	}
	
	// Log
	public void Log(object msg) {
		writer.WriteLine(System.DateTime.UtcNow.ToString(timeFormat) + ": " + msg);
#if UNITY_EDITOR
		if(useUnityDebugLog)
			Debug.Log(string.Concat("<color=" + timeColor + ">", System.DateTime.UtcNow.ToString(timeFormat), ":</color> <color=" + messageColor + ">", msg.ToString().Replace("\n", "</color>\n<color=" + messageColor + ">"), "</color>"));
#endif
	}
	
	// LogWarning
	public void LogWarning(object msg) {
		writer.WriteLine(System.DateTime.UtcNow.ToString(timeFormat) + ": [WARNING] " + msg);
#if UNITY_EDITOR
		if(useUnityDebugLog)
			Debug.LogWarning(string.Concat("<color=" + timeColor + ">", System.DateTime.UtcNow.ToString(timeFormat), ":</color> <color=" + messageColor + ">", msg.ToString().Replace("\n", "</color>\n<color=" + messageColor + ">"), "</color>"));
#endif
	}
	
	// LogError
	public void LogError(object msg) {
		writer.WriteLine(System.DateTime.UtcNow.ToString(timeFormat) + ": [ERROR] " + msg);
#if UNITY_EDITOR
		if(useUnityDebugLog)
			Debug.LogError(string.Concat("<color=" + timeColor + ">", System.DateTime.UtcNow.ToString(timeFormat), ":</color> <color=" + messageColor + ">", msg.ToString().Replace("\n", "</color>\n<color=" + messageColor + ">"), "</color>"));
#endif
	}
	
	// Close
	public void Close() {
		writer.Close();
	}
	
	// GenerateReport
	public void GenerateReport() {
		Log("Build date: " + Utility.GetBuildDate());
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