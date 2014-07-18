using UnityEngine;

public class CrossHair : HUDElement {
	public Texture2D tex;
	public int defaultWidth;
	public int defaultHeight;
	public Color defaultColor;
	
	[HideInInspector]
	public Color color;
	
	private int halfWidth;
	private int halfHeight;
	private int _width;
	private int _height;
	
	// Start
	void Start() {
		color = defaultColor;
		width = defaultWidth;
		height = defaultHeight;
	}

	// Update
	void Update() {
		var vec = CameraMode.current.GetCursorPosition3D();
		relativeDimensions.x = vec.x;
		relativeDimensions.y = 1f - vec.y;
		UpdateDimensions();
	}
	
	// Draw
	public override void Draw() {
		GUI.color = this.color;
		GUI.DrawTexture(new Rect(GUIArea.width / 2 - halfWidth, GUIArea.height / 2 - halfHeight, width, height), tex);
	}
	
	// Width
	public int width {
		get {
			return _width;
		}

		set {
			if(_width == value)
				return;
			
			_width = value;
			halfWidth = _width / 2;
		}
	}
	
	// Height
	public int height {
		get { return _height; }
		set {
			if(_height == value)
				return;
			
			_height = value;
			halfHeight = _height / 2;
		}
	}
}
