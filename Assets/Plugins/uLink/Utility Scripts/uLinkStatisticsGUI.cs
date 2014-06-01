// (c)2011 MuchDifferent. All Rights Reserved.

using System;
using System.Globalization;
using UnityEngine;
using uLink;

/// <summary>
/// A graphical tool for the game client. Perfect for testers.
/// </summary>
/// <remarks>
/// Add this script component to one of the game objects. After that, testers will be able to
/// press the "enabledByKey" and bring up a window showing some important numbers from uLink:
/// ping time, bandwidth in both directions, number of networkViews (objects), and more.
/// </remarks>

[AddComponentMenu("uLink Utilities/Statistics GUI")]
public class uLinkStatisticsGUI : uLink.MonoBehaviour
{
	public enum Position
	{
		BottomLeft,
		BottomRight,
		TopLeft,
		TopRight,
	}

	public Position position = Position.TopLeft;

	public bool showOnlyInEditor = false;

	public KeyCode enabledByKey = KeyCode.Tab;
	public bool isEnabled = true;

	public bool dontDestroyOnLoad = false;

	public GUISkin guiSkin = null;
	public int guiDepth = 0;

	public bool showDetails = false;

	private const float WINDOW_MARGIN_X = 10;
	private const float WINDOW_MARGIN_Y = 10;

	private const float WINDOW_WDITH = 300;
	private const float COLUMN_WIDTH = 150;

	void Awake()
	{
		if (dontDestroyOnLoad) DontDestroyOnLoad(this);
	}

	void Update()
	{
		if (enabledByKey != KeyCode.None && Input.GetKeyDown(enabledByKey)) isEnabled = !isEnabled;
	}

	void OnGUI()
	{
		if (!isEnabled || (showOnlyInEditor && !Application.isEditor)) return;

		var oldSkin = GUI.skin;
		var oldDepth = GUI.depth;

		GUI.skin = guiSkin;
		GUI.depth = guiDepth;

		DrawGUI(position, showDetails);

		GUI.skin = oldSkin;
		GUI.depth = oldDepth;
	}

	public static void DrawGUI()
	{
		DrawGUI(Position.TopLeft, false);
	}

	public static void DrawGUI(Position position)
	{
		DrawGUI(position, false);
	}

	public static void DrawGUI(bool showDetails)
	{
		DrawGUI(Position.TopLeft, showDetails);
	}

	public static void DrawGUI(Position position, bool showDetails)
	{
		uLink.NetworkPlayer[] connections = uLink.Network.connections;

		GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));

		GUILayout.BeginHorizontal();
		if (position == Position.TopLeft || position == Position.BottomLeft)
		{
			GUILayout.Space(WINDOW_MARGIN_X);
		}
		else
		{
			GUILayout.FlexibleSpace();
		}

		GUILayout.BeginVertical();
		if (position == Position.TopLeft || position == Position.TopRight)
		{
			GUILayout.Space(WINDOW_MARGIN_Y);
		}
		else
		{
			GUILayout.FlexibleSpace();
		}

		GUILayout.BeginVertical(GUILayout.Width(WINDOW_WDITH));

		GUILayout.BeginVertical(GUI.skin.box);

		GUILayout.BeginHorizontal();
		GUILayout.Label("Frame Rate:", GUILayout.Width(COLUMN_WIDTH));
		GUILayout.Label(Mathf.RoundToInt(1.0f / Time.smoothDeltaTime).ToString(CultureInfo.InvariantCulture) + " FPS");
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Status:", GUILayout.Width(COLUMN_WIDTH));
		GUILayout.Label(uLink.NetworkUtility.GetStatusString(uLink.Network.peerType, uLink.Network.status));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Last Error:", GUILayout.Width(COLUMN_WIDTH));
		GUILayout.Label(uLink.NetworkUtility.GetErrorString(uLink.Network.lastError));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Network Time:", GUILayout.Width(COLUMN_WIDTH));
		GUILayout.Label(uLink.Network.time.ToString(CultureInfo.InvariantCulture) + " s");
		GUILayout.EndHorizontal();

		if (showDetails)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Server Time Offset:", GUILayout.Width(COLUMN_WIDTH));
			GUILayout.Label((uLink.Network.config.serverTimeOffsetInMillis * 0.001).ToString(CultureInfo.InvariantCulture) + " s");
			GUILayout.EndHorizontal();
		}

		GUILayout.BeginHorizontal();
		GUILayout.Label("Network Objects:", GUILayout.Width(COLUMN_WIDTH));
		GUILayout.Label(uLink.Network.networkViewCount.ToString(CultureInfo.InvariantCulture));
		GUILayout.EndHorizontal();

		if (uLink.Network.isServer)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Connections:", GUILayout.Width(COLUMN_WIDTH));
			GUILayout.Label(connections.Length.ToString(CultureInfo.InvariantCulture));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Name in Master Server:", GUILayout.Width(COLUMN_WIDTH));
			GUILayout.Label(uLink.MasterServer.isRegistered ? uLink.MasterServer.gameName : "Not Registered");
			GUILayout.EndHorizontal();
		}

		GUILayout.EndVertical();

		foreach (var player in connections)
		{
			uLink.NetworkStatistics stats = player.statistics;
			if (stats == null) continue;

			GUILayout.BeginVertical("Box");

			GUILayout.BeginHorizontal();
			GUILayout.Label("Connection:", GUILayout.Width(COLUMN_WIDTH));
			GUILayout.Label(player.ToString());
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Ping (average):", GUILayout.Width(COLUMN_WIDTH));
			GUILayout.Label(player.lastPing + " (" + player.averagePing + ") ms");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Sent:", GUILayout.Width(COLUMN_WIDTH));
			GUILayout.Label((int) Math.Round(stats.bytesSentPerSecond) + " B/s");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Receive:", GUILayout.Width(COLUMN_WIDTH));
			GUILayout.Label((int) Math.Round(stats.bytesReceivedPerSecond) + " B/s");
			GUILayout.EndHorizontal();

			if (showDetails)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Packets sent:", GUILayout.Width(COLUMN_WIDTH));
				GUILayout.Label(stats.packetsSent.ToString(CultureInfo.InvariantCulture));
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Packets received:", GUILayout.Width(COLUMN_WIDTH));
				GUILayout.Label(stats.packetsReceived.ToString(CultureInfo.InvariantCulture));
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Messages sent:", GUILayout.Width(COLUMN_WIDTH));
				GUILayout.Label(stats.messagesSent.ToString(CultureInfo.InvariantCulture));
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Messages received:", GUILayout.Width(COLUMN_WIDTH));
				GUILayout.Label(stats.messagesReceived.ToString(CultureInfo.InvariantCulture));
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Messages resent:", GUILayout.Width(COLUMN_WIDTH));
				GUILayout.Label(stats.messagesResent.ToString(CultureInfo.InvariantCulture));
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Messages unsent:", GUILayout.Width(COLUMN_WIDTH));
				GUILayout.Label(stats.messagesUnsent.ToString(CultureInfo.InvariantCulture));
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Messages stored:", GUILayout.Width(COLUMN_WIDTH));
				GUILayout.Label(stats.messagesStored.ToString(CultureInfo.InvariantCulture));
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Messages withheld:", GUILayout.Width(COLUMN_WIDTH));
				GUILayout.Label(stats.messagesWithheld.ToString(CultureInfo.InvariantCulture));
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Msg duplicates rejected:", GUILayout.Width(COLUMN_WIDTH));
				GUILayout.Label(stats.messageDuplicatesRejected.ToString(CultureInfo.InvariantCulture));
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Msg sequences rejected:", GUILayout.Width(COLUMN_WIDTH));
				GUILayout.Label(stats.messageSequencesRejected.ToString(CultureInfo.InvariantCulture));
				GUILayout.EndHorizontal();
			}

			GUILayout.BeginHorizontal();
			GUILayout.Label("Encryption:", GUILayout.Width(COLUMN_WIDTH));
			GUILayout.Label(player.hasSecurity ? "On" : "Off");
			GUILayout.EndHorizontal();

			GUILayout.EndVertical();
		}

		GUILayout.EndVertical();


		if (position != Position.TopLeft && position != Position.TopRight)
		{
			GUILayout.Space(WINDOW_MARGIN_Y);
		}
		else
		{
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndVertical();

		if (position != Position.TopLeft && position != Position.BottomLeft)
		{
			GUILayout.Space(WINDOW_MARGIN_X);
		}
		else
		{
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndHorizontal();

		GUILayout.EndArea();
	}
}
