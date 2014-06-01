using UnityEngine;
using System.Collections;

public class GUIColor {
	public static Color Shadow = new Color(0f, 0f, 0f, 0.85f);
	
	public static Color StatusMessage = Color.yellow;
	public static Color Validated = Color.green;
	public static Color NotValidated = Color.white;
	
	public static Color MenuItemActive = Color.blue;
	public static Color MenuItemLoading = Color.cyan;
	public static Color MenuItemInactive = Color.white;
	
	public static Color RankingMine = Color.white;
	public static Color RankingOther = Color.white;
	
	public static Color RankingBGMine = new Color(1f, 1f, 1f, 1f);
	public static Color RankingBGOther = new Color(1f, 1f, 1f, 0.5f);
	
	public static Color GetChannelColor(string channel) {
		switch(channel) {
			case "Global":
				return Color.white;
			case "Announcement":
				return Color.cyan;
			case "Map":
				return new Color(1.0f, 0.85f, 0.6f, 1f);
			case "System":
				return new Color(1f, 1f, 0.5f, 1f);
			default:
				return Color.white;
		}
	}
}
