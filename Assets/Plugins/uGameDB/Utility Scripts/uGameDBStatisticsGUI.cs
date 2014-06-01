// (c)2011 MuchDifferent. All Rights Reserved.

using UnityEngine;
using uGameDB;

/// <summary>
/// This script places a GUI box at the left of your screen that prints some useful information about the uGameDB runtime.
/// </summary>
public class uGameDBStatisticsGUI : MonoBehaviour
{
	private const int WindowMargin = 10;

	public void OnGUI()
	{
		GUI.depth = 0;
		GUILayout.BeginArea(new Rect(WindowMargin, WindowMargin, Screen.width - 2 * WindowMargin,
									 Screen.height - 2 * WindowMargin));
		DrawStatisticsBox();
		GUILayout.EndArea();
	}

	/// <summary>
	/// Use this method to place a uGameDB statistics box inside your own GUI. The box has a fixed width
	/// but grows downwards to fit the contents.
	/// </summary>
	/// <param name="width">The width of the bo in pixels. The default is 300 px.</param>
	public static void DrawStatisticsBox(int width = 300)
	{
		GUILayout.BeginVertical("Box", GUILayout.Width(width));

		GUILayout.Label("uGameDB Statistics");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Status:", GUILayout.Width(width / 2));
		GUILayout.Label(Database.isConnected ? "Connected" : "Not Connected");
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Pending Requests:", GUILayout.Width(width / 2));
		GUILayout.Label(Database.pendingRequestCount.ToString());
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Queued Requests:", GUILayout.Width(width / 2));
		GUILayout.Label(Database.queuedRequestCount.ToString());
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Nodes (online/total):", GUILayout.Width(width / 2));
		GUILayout.Label(Database.onlineNodeCount + "/" + Database.nodeCount);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("VClock cache hit ratio:", GUILayout.Width(width / 2));
		GUILayout.Label(Database.vectorClockCacheHitRatio.ToString("F2") + " (" + Database.vectorClockCacheHits + " hits, " + Database.vectorClockCacheMisses + " misses)");
		GUILayout.EndHorizontal();

		GUILayout.EndVertical();
	}
}
