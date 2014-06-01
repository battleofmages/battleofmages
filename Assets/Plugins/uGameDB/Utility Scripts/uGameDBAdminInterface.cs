// (c)2011 MuchDifferent. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using Jboy;
using UnityEngine;
using uGameDB;

/// <summary>
/// This component displays a Riak admin interface that allows you to list and
/// see the contents of the whole database cluster. This script should not be
/// used for large-scale purposes as it fetches the whole dataset before it
/// displays the results.
/// </summary>
public class uGameDBAdminInterface : MonoBehaviour
{
	private const string InfoText =
@"This is a simple example of how you can implement a Riak admin interface in Unity using uGameDB.

The uGameDBAdminInterface component draws this GUI and allows you to list and see the full contents of the database. You are also able to remove specific entries and clear (remove) whole buckets. Note that this admin tool is not recommended for large-scale databases as the amount of data that is retrieved can be huge.

To the right you can see some information about the uGameDB client. This is the same data that you get on-screen if you use the uGameDBStatisticsGUI utility script.

Below you can see the list of buckets and keys in the database. Please refer to the uGameDBAdminInterface.cs file for more information.";

	public bool ShowInfoHeader = true;
	public int JsonIndentSize = 25;

	private const int WindowMargin = 10;
	private bool _showInfo = true;
	private Vector2 _scrollPosInfo;
	private Vector2 _scrollPosData;
	private List<BucketData> _buckets = new List<BucketData>();
	private bool _enableRefresh = true;

	/// <summary>
	/// This is a local representation of one key/value pair from the database.
	/// </summary>
	private class Entry
	{
		public Bucket Bucket;
		public string Key;
		public object Value;
		public bool Expanded;
	}

	/// <summary>
	/// This is a local representation of one whole bucket from the database.
	/// </summary>
	private class BucketData
	{
		public Bucket Bucket;
		public List<Entry> Entries;
	}

	public void Start()
	{
		// Refresh the list once on start.
		StartCoroutine(RefreshList());
	}

	private IEnumerator RefreshList()
	{
		// Only allow a new refresh if the previous one has finished.
		if (!_enableRefresh) yield break;
		_enableRefresh = false;

		// If the uGameDB is not connected to Riak, wait and try again.
		while (!Database.isConnected) yield return new WaitForSeconds(0.5f);

		// Refresh the buckets and wait for finish.
		yield return StartCoroutine(RefreshBuckets());

		// We are done, a new refresh can be started.
		_enableRefresh = true;
	}

	private IEnumerator RefreshBuckets()
	{
		// Build a separate bucket list until the operation finishes.
		var newBuckets = new List<BucketData>();

		// Get a list of all buckets
		var getBucketsReq = Bucket.GetBuckets();
		yield return getBucketsReq.WaitUntilDone();
		if (getBucketsReq.isSuccessful)
		{
			foreach (var bucket in getBucketsReq.GetBucketEnumerable())
			{
				var bucketData = new BucketData { Bucket = bucket };

				// Get a list of all keys in the bucket.
				var getKeysReq = bucket.GetKeys();
				yield return getKeysReq.WaitUntilDone();
				if (!getKeysReq.isSuccessful) continue;

				// Build a new list of keys in the bucket.
				bucketData.Entries = new List<Entry>();
				foreach (var key in getKeysReq.GetKeyEnumerable())
				{
					var entry = new Entry { Bucket = bucket, Key = key, Value = "[unknown data]" };

					// Get the value for the key
					var getReq = bucket.Get(key);
					yield return getReq.WaitUntilDone();
					if (getReq.isSuccessful)
					{
						// Try to deserialize the data to a string. If this does not work, use the raw data.
						if (getReq.GetEncoding() == Encoding.Json)
							entry.Value = getReq.GetValue<JsonTree>();
						else
							entry.Value = getReq.GetValueRaw();
					}
					bucketData.Entries.Add(entry);
				}
				// Sort the key/value pairs by key.
				bucketData.Entries.Sort((a, b) => a.Key.CompareTo(b.Key));
				newBuckets.Add(bucketData);
			}
			// Sort the buckets by their name.
			newBuckets.Sort((a, b) => a.Bucket.name.CompareTo(b.Bucket.name));
		}
		_buckets = newBuckets;
	}

	private IEnumerator Remove(Bucket bucket, string key)
	{
		// Remove one entry from the database.
		var removeRequest = bucket.Remove(key);
		yield return removeRequest.WaitUntilDone();
		if (removeRequest.hasFailed)
			Debug.LogError("Unable to remove '" + removeRequest.key + "' from " + removeRequest.bucket + ". " +
						   removeRequest.GetErrorString());

		// Refresh the list once the request has completed.
		StartCoroutine(RefreshList());
	}

	private IEnumerator ClearBucket(Bucket bucket)
	{
		// Get all keys in the given bucket.
		var getKeysRequest = bucket.GetKeys();
		yield return getKeysRequest.WaitUntilDone();
		if (getKeysRequest.hasFailed)
		{
			Debug.LogError("Unable to clear " + bucket + ". " + getKeysRequest.GetErrorString());
			yield break;
		}

		// Remove each one of the keys. Issue the requests all at once.
		var removeRequests = new List<RemoveRequest>();
		foreach (var key in getKeysRequest.GetKeyEnumerable())
		{
			removeRequests.Add(bucket.Remove(key));
		}

		// Then wait for each request to finish.
		foreach (var removeRequest in removeRequests)
		{
			yield return removeRequest.WaitUntilDone();
			if (removeRequest.hasFailed)
				Debug.LogError("Unable to remove '" + removeRequest.key + "' from " + removeRequest.bucket + ". " +
							   removeRequest.GetErrorString());
		}

		// Finally refresh the list.
		StartCoroutine(RefreshList());
	}

	///////////////////////////////////////////////////////////////////////////
	// GUI
	///////////////////////////////////////////////////////////////////////////

	public void OnGUI()
	{
		// Wrap the whole screen in an area object with a margin to the window edge.
		GUI.depth = 0;
		GUILayout.BeginArea(new Rect(WindowMargin, WindowMargin, Screen.width - 2 * WindowMargin,
									 Screen.height - 2 * WindowMargin));
		DrawAdminInterfaceGUI();
		GUILayout.EndArea();
	}

	void DrawAdminInterfaceGUI()
	{
		GUILayout.BeginVertical();

		if (ShowInfoHeader) DrawInfoArea();
		DrawAdminArea();

		GUILayout.EndVertical();
	}

	private void DrawInfoArea()
	{
		GUILayout.BeginVertical();

		// The info area at the top can be shown or hidden.
		if (GUILayout.Button("Toggle Info")) _showInfo = !_showInfo;
		if (_showInfo)
		{
			GUILayout.BeginHorizontal("Box", GUILayout.Height(1));

			_scrollPosInfo = GUILayout.BeginScrollView(_scrollPosInfo);
			GUILayout.TextArea(InfoText, "Label");
			GUILayout.EndScrollView();

			uGameDBStatisticsGUI.DrawStatisticsBox(250);

			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
	}

	private void DrawAdminArea()
	{
		GUILayout.BeginVertical();

		DrawDataList();

		if (GUILayout.Button("Refresh")) StartCoroutine(RefreshList());

		GUILayout.EndVertical();
	}

	private void DrawDataList()
	{
		GUILayout.BeginVertical("Box");

		GUILayout.Label("Database Contents");

		if (_buckets.Count == 0)
		{
			// If the database is empty, just display a centered message.
			GUILayout.BeginVertical("Box");
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Database is empty");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
		}
		else
		{
			// If there are items, draw them inside a scroll view.
			_scrollPosData = GUILayout.BeginScrollView(_scrollPosData);
			foreach (var b in _buckets)
			{
				DrawBucket(b);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndScrollView();
		}

		GUILayout.EndVertical();
	}

	private void DrawBucket(BucketData b)
	{
		GUILayout.BeginHorizontal("box");
		GUILayout.BeginVertical();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Bucket: " + b.Bucket.name);
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Clear")) StartCoroutine(ClearBucket(b.Bucket));
		GUILayout.EndHorizontal();

		if (b.Entries != null)
		{
			foreach (var pair in b.Entries)
			{
				DrawKeyValue(pair);
			}
		}
		else
		{
			DrawUnknownKeyValue();
		}

		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}

	private void DrawKeyValue(Entry entry)
	{
		GUILayout.BeginHorizontal("box");

		GUILayout.Label("Key: " + entry.Key, GUILayout.Width(Screen.width / 4f));

		if (entry.Value is JsonTree)
		{
			if (GUILayout.Button(entry.Expanded ? "-" : "+")) entry.Expanded = !entry.Expanded;
			if (entry.Expanded)
				DrawJsonDataValueExpanded((JsonTree) entry.Value);
			else
				DrawJsonDataValueCollapsed((JsonTree) entry.Value);
		}
		else
			GUILayout.Label("Value: " + entry.Value);
		GUILayout.FlexibleSpace();

		if (GUILayout.Button("Remove")) StartCoroutine(Remove(entry.Bucket, entry.Key));

		GUILayout.EndHorizontal();
	}

	private void DrawJsonDataValueCollapsed(JsonTree jsonTree)
	{
		GUILayout.BeginHorizontal();
		GUILayout.TextField(jsonTree.ToString(), "Label");
		GUILayout.EndHorizontal();
	}

	private void DrawJsonDataValueExpanded(JsonTree jsonTree)
	{
		if (jsonTree.IsObject)
		{
			GUILayout.BeginVertical();
			GUILayout.Label("{");
			foreach (var prop in jsonTree.AsObject)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space(JsonIndentSize);
				GUILayout.Label(prop.Name);
				GUILayout.Label(":");
				DrawJsonDataValueExpanded(prop.Value);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
			GUILayout.Label("}");
			GUILayout.EndVertical();
		}
		else if (jsonTree.IsArray)
		{
			GUILayout.BeginVertical();
			GUILayout.Label("[");
			foreach (var item in jsonTree.AsArray)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space(JsonIndentSize);
				DrawJsonDataValueExpanded(item);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
			GUILayout.Label("]");
			GUILayout.EndVertical();
		}
		else
		{
			GUILayout.BeginHorizontal();
			GUILayout.TextField(jsonTree.ToString(), "Label");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
	}

	private void DrawUnknownKeyValue()
	{
		GUILayout.BeginHorizontal("box");

		GUILayout.Label("[unknown keys/values]");
		GUILayout.FlexibleSpace();

		GUILayout.EndHorizontal();
	}
}
