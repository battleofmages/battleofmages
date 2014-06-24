using UnityEngine;

public class HUDElement : MonoBehaviour {
	public static bool editMode = false;
	
	// DO NOT MODIFY THESE WITHOUT CALLING UpdateDimensions
	public Rect relativeDimensions;
	public Rect absoluteDimensions;
	public int guiDepth;
	
	protected Rect dimensions;
	private Vector2 oldScreenSize;
	
	// Awake
	void Awake() {
		UpdateDimensions();
	}
	
	// OnGUI
	public void OnGUI() {
		if(!uLink.Network.isClient)
			return;
		
		GUI.depth = guiDepth;
		
		if(oldScreenSize.x != GUIArea.width || oldScreenSize.y != GUIArea.height)
			UpdateDimensions();
		
		using(new GUIArea(dimensions)) {
			if(editMode)
				DrawEditControls();
			else
				Draw();
		}
	}
	
	// UpdateDimensions
	public void UpdateDimensions() {
		dimensions.Set(
			absoluteDimensions.x + relativeDimensions.x * GUIArea.width,
			absoluteDimensions.y + relativeDimensions.y * GUIArea.height,
			absoluteDimensions.width + relativeDimensions.width * GUIArea.width,
			absoluteDimensions.height + relativeDimensions.height * GUIArea.height
		);
		
		oldScreenSize.x = GUIArea.width;
		oldScreenSize.y = GUIArea.height;
	}
	
	// DrawEditControls
	void DrawEditControls() {
		GUI.color = Color.green;
		using(new GUIVertical("box")) {
			GUILayout.Label(this.GetType().ToString());
			
			using(new GUIHorizontal()) {
				GUIHelper.RectPicker(ref relativeDimensions);
			}
			
			using(new GUIHorizontal()) {
				GUIHelper.RectPicker(ref absoluteDimensions);
			}
			
			GUILayout.FlexibleSpace();
		}
		GUI.color = Color.white;
		
	}
	
	public virtual void Draw() {}
}
