using UnityEngine;

[System.Serializable]
public class TextPhase {
	public string text;
	public bool overwriteTextStyle;
	public GUIStyle textStyle;
	public Vector2 position;
	public float start;
	public float duration;
	
	public float fadeInDuration;
	public float fadeOutDuration;
}