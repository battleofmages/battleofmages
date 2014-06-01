#if UNITY_STANDALONE_WIN
//#define FIX_APPLICATION_TARGETFRAMERATE
#endif

using System;
using UnityEngine;
using System.Runtime.InteropServices;

[AddComponentMenu("")]
[ExecuteInEditMode]
public class uLinkTargetFrameRateFix : MonoBehaviour
{
#if UNITY_STANDALONE_WIN
	const float BONUS_TICKS = 2.5f; // give extra ticks to more accurately balance frame time

	const uint QS_ALLEVENTS = 0x04BF;

	static uint prevTicksSinceStartup;
	static int prevSleepTicks;
	static int targetDeltaTicks;
	static uLinkTargetFrameRateFix singleton;

	[DllImport("User32.dll")]
	static extern uint MsgWaitForMultipleObjectsEx(uint nCount, IntPtr[] pHandles, uint dwMilliseconds, uint dwWakeMask, uint dwFlags);

	[DllImport("Winmm.dll")]
	static extern uint timeGetTime();
	
	void Awake()
	{
		if (singleton != null)
		{
			DestroyImmediate(this);
			return;
		}

		singleton = this;

		DontDestroyOnLoad(this);

		prevTicksSinceStartup = timeGetTime();
	}
	
	void LateUpdate()
	{
		if (Application.isPlaying)
		{
			uint curTicksSinceStartup = timeGetTime();
			int prevFrameTicks = (int)unchecked(curTicksSinceStartup - prevTicksSinceStartup);
			prevTicksSinceStartup = curTicksSinceStartup;

			int workTicks = prevFrameTicks - prevSleepTicks;
			int sleepTicks = targetDeltaTicks - workTicks;

			if (sleepTicks < 0 && prevSleepTicks < 0) sleepTicks = 0;
			prevSleepTicks = sleepTicks;

			if (sleepTicks > 0) MsgWaitForMultipleObjectsEx(0, null, (uint)sleepTicks, QS_ALLEVENTS, 0);
		}
		else if (Application.isEditor)
		{
			gameObject.hideFlags = 0;
			DestroyImmediate(gameObject);
		}
	}
	
	public static void SetTargetFrameRate(int frameRate)
	{
		if (QualitySettings.vSyncCount != 0) return;

		UnityEngine.Application.targetFrameRate = -1;

		if (UnityEngine.Application.platform != RuntimePlatform.WindowsPlayer)
		{
			UnityEngine.Application.targetFrameRate = frameRate;
			return;
		}

		try // make sure there is no issues calling these two native Win32 APIs
		{
			timeGetTime();
			MsgWaitForMultipleObjectsEx(0, null, 0, 0, 0);
		}
		catch (Exception)
		{
			UnityEngine.Application.targetFrameRate = frameRate;
			return;
		}

		if (frameRate == 0 || frameRate == -1)
		{
			if (singleton != null)
			{
				DestroyImmediate(singleton);
				singleton = null;
			}

			targetDeltaTicks = 0;
			return;
		}

		targetDeltaTicks = Mathf.RoundToInt(1000f / frameRate + BONUS_TICKS);
		
		if (singleton != null) return;

		var go = new GameObject("uLinkTargetFrameRateFix", typeof(uLinkTargetFrameRateFix));
		go.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable | HideFlags.DontSave;
	}
#else
	public static void SetTargetFrameRate(int frameRate)
	{
		if (QualitySettings.vSyncCount != 0) return;

		UnityEngine.Application.targetFrameRate = frameRate;
	}
#endif
}

#if FIX_APPLICATION_TARGETFRAMERATE
public static class Application
{
	static int _targetFrameRate;

	public static int targetFrameRate
	{
		get
		{
			return _targetFrameRate;
		}
		set
		{
			_targetFrameRate = value;

			if (uLinkTargetFrameRateFix.SetTargetFrameRate(_targetFrameRate))
			{
				Debug.Log("Overridden Application.targetFrameRate = " + _targetFrameRate + " with uLink's low CPU usage fix.");
			}
			else
			{
				UnityEngine.Application.targetFrameRate = _targetFrameRate;
			}
			
		} 
	}

	public static void CancelQuit() { UnityEngine.Application.CancelQuit(); }
	public static bool CanStreamedLevelBeLoaded(int levelIndex) { return UnityEngine.Application.CanStreamedLevelBeLoaded(levelIndex); }
	public static bool CanStreamedLevelBeLoaded(string levelName) { return UnityEngine.Application.CanStreamedLevelBeLoaded(levelName); }
	public static void CaptureScreenshot(string filename) { UnityEngine.Application.CaptureScreenshot(filename); }
	public static void ExternalCall(string functionName, params object[] args) { UnityEngine.Application.ExternalCall(functionName, args); }
	public static void ExternalEval(string script) { UnityEngine.Application.ExternalCall(script); }
	public static float GetStreamProgressForLevel(int levelIndex) { return UnityEngine.Application.GetStreamProgressForLevel(levelIndex); }
	public static float GetStreamProgressForLevel(string levelName) { return UnityEngine.Application.GetStreamProgressForLevel(levelName); }
	public static void LoadLevel(int index) { UnityEngine.Application.LoadLevel(index); }
	public static void LoadLevel(string name) { UnityEngine.Application.LoadLevel(name); }
	public static void LoadLevelAdditive(int index) { UnityEngine.Application.LoadLevelAdditive(index); }
	public static void LoadLevelAdditive(string name) { UnityEngine.Application.LoadLevelAdditive(name); }
	public static AsyncOperation LoadLevelAdditiveAsync(int index) { return UnityEngine.Application.LoadLevelAdditiveAsync(index); }
	public static AsyncOperation LoadLevelAdditiveAsync(string levelName) { return UnityEngine.Application.LoadLevelAdditiveAsync(levelName); }
	public static AsyncOperation LoadLevelAsync(int index) { return UnityEngine.Application.LoadLevelAsync(index); }
	public static AsyncOperation LoadLevelAsync(string levelName) { return UnityEngine.Application.LoadLevelAsync(levelName); }
	public static void OpenURL(string url) { UnityEngine.Application.OpenURL(url); }
	public static void Quit() { UnityEngine.Application.Quit(); }
	public static void RegisterLogCallback(UnityEngine.Application.LogCallback handler) { UnityEngine.Application.RegisterLogCallback(handler); }
	public static void RegisterLogCallbackThreaded(UnityEngine.Application.LogCallback handler) { UnityEngine.Application.RegisterLogCallbackThreaded(handler); }
	public static string absoluteURL { get { return UnityEngine.Application.absoluteURL; } }
	public static ThreadPriority backgroundLoadingPriority { get { return UnityEngine.Application.backgroundLoadingPriority; } set { UnityEngine.Application.backgroundLoadingPriority = value; } }
	public static string dataPath { get { return UnityEngine.Application.dataPath; } }
	public static bool isEditor { get { return UnityEngine.Application.isEditor; } }
	public static bool isLoadingLevel { get { return UnityEngine.Application.isLoadingLevel; } }
	public static bool isPlaying { get { return UnityEngine.Application.isPlaying; } }
	public static bool isWebPlayer { get { return UnityEngine.Application.isWebPlayer; } }
	public static int levelCount { get { return UnityEngine.Application.levelCount; } }
	public static int loadedLevel { get { return UnityEngine.Application.loadedLevel; } }
	public static string loadedLevelName { get { return UnityEngine.Application.loadedLevelName; } }
	public static string persistentDataPath { get { return UnityEngine.Application.persistentDataPath; } }
	public static RuntimePlatform platform { get { return UnityEngine.Application.platform; } }
	public static bool runInBackground { get { return UnityEngine.Application.runInBackground; } set { UnityEngine.Application.runInBackground = value; } }
	public static string srcValue { get { return UnityEngine.Application.srcValue; } }
	public static int streamedBytes { get { return UnityEngine.Application.streamedBytes; } }
	public static SystemLanguage systemLanguage { get { return UnityEngine.Application.systemLanguage; } }
	public static string temporaryCachePath { get { return UnityEngine.Application.temporaryCachePath; } }
	public static string unityVersion { get { return UnityEngine.Application.unityVersion; } }
	public static bool webSecurityEnabled { get { return UnityEngine.Application.webSecurityEnabled; } }
	public static string webSecurityHostUrl { get { return UnityEngine.Application.webSecurityHostUrl; } }
	public static NetworkReachability internetReachability { get { return UnityEngine.Application.internetReachability; } }
#if UNITY_3_4_2 || UNITY_3_5
	public static bool HasUserAuthorization(UserAuthorization mode) { return UnityEngine.Application.HasUserAuthorization(mode); }
	public static AsyncOperation RequestUserAuthorization(UserAuthorization mode) { return UnityEngine.Application.RequestUserAuthorization(mode); }
	public static bool genuine { get { return UnityEngine.Application.genuine; } }
	public static bool genuineCheckAvailable { get { return UnityEngine.Application.genuineCheckAvailable; } }
#if UNITY_3_5
	public static void CaptureScreenshot(string filename, int superSize) { UnityEngine.Application.CaptureScreenshot(filename, superSize); }
	public static string streamingAssetsPath { get { return UnityEngine.Application.streamingAssetsPath; } }
#endif
#endif
}
#endif
