// (c)2011 MuchDifferent. All Rights Reserved.

using UnityEngine;
using uLink;

[AddComponentMenu("uLink Utilities/Object Label")]
public class uLinkObjectLabel : uLink.MonoBehaviour
{
	public GUIText prefabLabel;
	
	public bool useInitialData = false;
	
	public float minDistance = 1;
	public float maxDistance = 500;

	public Vector3 offset = new Vector3(0, 2, 0);    // Units in world space to offset; 1 unit above object by default

	public bool clampToScreen = false;  // If true, label will be visible even if object is off screen
	public float clampBorderSize = 0.05f;  // How much viewport space to leave at the borders when a label is being clamped
	
	public Color color = Color.white;
	
	public bool manualUpdate = false;
	
	private GUIText instantiatedLabel;

	void uLink_OnNetworkInstantiate(uLink.NetworkMessageInfo info)
	{
		if (!enabled) return;
		
		string text;
		
		if (!useInitialData)
		{
			text = info.networkView.owner.ToString();
		}
		if (!info.networkView.initialData.TryRead<string>(out text))
		{
			// TODO: log error
			return;
		}
		
		instantiatedLabel = (GUIText) Instantiate(prefabLabel, Vector3.zero, Quaternion.identity);
		instantiatedLabel.text = text;
		instantiatedLabel.material.color = color;
	}

	void OnDisable()
	{
		if (instantiatedLabel != null)
		{
			DestroyImmediate(instantiatedLabel.gameObject);
			instantiatedLabel = null;
		}
	}

	void LateUpdate()
	{
		if (manualUpdate) return;

		ManualUpdate();
	}
	
	public void ManualUpdate()
	{
		if (instantiatedLabel == null || Camera.main == null) return;
		
		Vector3 pos;

		if (clampToScreen)
		{
			Vector3 rel = Camera.main.transform.InverseTransformPoint(transform.position);
			rel.z = Mathf.Max(rel.z, 1.0f);

			pos = Camera.main.WorldToViewportPoint(Camera.main.transform.TransformPoint(rel + offset));
			pos = new Vector3(
				Mathf.Clamp(pos.x, clampBorderSize, 1.0f - clampBorderSize),
				Mathf.Clamp(pos.y, clampBorderSize, 1.0f - clampBorderSize),
				pos.z);
		}
		else
		{
			pos = Camera.main.WorldToViewportPoint(transform.position + offset);
		}

		instantiatedLabel.transform.position = pos;
		instantiatedLabel.enabled = (pos.z >= minDistance && pos.z <= maxDistance);
	}
	
	public static void ManualUpdateAll()
	{
		uLinkObjectLabel[] labels = (uLinkObjectLabel[]) FindObjectsOfType(typeof(uLinkObjectLabel));
		
		foreach (uLinkObjectLabel label in labels)
		{
			label.ManualUpdate();
		}
	}
}
