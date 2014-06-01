using uLink;
using UnityEngine;
using System.Collections;

public class EntityLabel : uLink.MonoBehaviour {
	public static Transform labelRoot;
	
	public GUIText prefabLabel;
	public Color initialColor = Color.white;
	public string initialText = "";
	public float minDistance = 0.1f;
	public Vector3 offset = new Vector3(0, 2, 0);
	
	// If true, label will be visible even if object is off screen
	public bool clampToScreen = false;
	
	// How much viewport space to leave at the borders when a label is being clamped
	public float clampBorderSize = 0.05f;
	
	protected GUIText instantiatedLabel;
	protected Transform labelTransform;
	
	// Awake
	void Awake() {
		if(!this.enabled)
			return;
		
		instantiatedLabel = (GUIText)Instantiate(prefabLabel, Cache.vector3Zero, Cache.quaternionIdentity);
		instantiatedLabel.text = initialText;
		instantiatedLabel.material.color = initialColor;
		instantiatedLabel.enabled = this.enabled;
		labelTransform = instantiatedLabel.transform;
		labelTransform.parent = labelRoot;
		labelTransform.name = instantiatedLabel.text;
	}
	
	// LateUpdate
	void LateUpdate() {
		if(instantiatedLabel == null || Camera.main == null || !this.enabled)
			return;
		
		Vector3 pos;

		if(clampToScreen) {
			Vector3 rel = Camera.main.transform.InverseTransformPoint(transform.position);
			rel.z = Mathf.Max(rel.z, 1.0f);

			pos = Camera.main.WorldToViewportPoint(Camera.main.transform.TransformPoint(rel + offset));
			pos = new Vector3(
				Mathf.Clamp(pos.x, clampBorderSize, 1.0f - clampBorderSize),
				Mathf.Clamp(pos.y, clampBorderSize, 1.0f - clampBorderSize),
				pos.z
			);
		} else {
			pos = Camera.main.WorldToViewportPoint(transform.position + offset);
		}

		labelTransform.position = pos;
		instantiatedLabel.enabled = (pos.z >= minDistance && pos.z <= Config.instance.entityVisibilityDistance);
	}
	
	// OnEnable
	void OnEnable() {
		if(instantiatedLabel != null)
			instantiatedLabel.enabled = true;
	}
	
	// OnDisable
	void OnDisable() {
		if(instantiatedLabel != null)
			instantiatedLabel.enabled = false;
	}
	
	// Text
	public string text {
		get {
			if(instantiatedLabel == null)
				return initialText;
			
			return instantiatedLabel.text;
		}
		
		set {
			if(instantiatedLabel == null) {
				initialText = value;
				return;
			}
			
			instantiatedLabel.text = value;
			labelTransform.name = value;
		}
	}
	
	// Text color
	public Color textColor {
		get {
			if(instantiatedLabel == null)
				return initialColor;
			
			return instantiatedLabel.material.color;
		}
		
		set {
			if(instantiatedLabel == null) {
				initialColor = value;
				return;
			}
			
			instantiatedLabel.material.color = value;
		}
	}
	
	// Rect
	public Rect rect {
		get {
			return instantiatedLabel.GetScreenRect();
		}
	}
}
