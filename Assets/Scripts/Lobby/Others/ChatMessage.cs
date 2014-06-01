using UnityEngine;

public class ChatMessage {
	public readonly string channel;
	public readonly string playerName;
	public readonly string text;
	public readonly TimeStamp timeStamp;
	
	public readonly string prettyString;
	public readonly GUIContent guiContent;
	
	public ChatMessage(string nChannel, string nPlayerName, string nText, TimeStamp nTimeStamp) {
		channel = nChannel;
		playerName = nPlayerName;
		text = nText;
		timeStamp = nTimeStamp;
		
		// Pretty string
		if(playerName != "") {
			prettyString = "[" + channel + "] <b>" + playerName + "</b>: " + text;
		} else {
			if(channel != "")
				prettyString = "[" + channel + "] " + text;
			else
				prettyString = text;
		}
		
		guiContent = new GUIContent(prettyString);
	}
	
	public Color color {
		get {
			return GUIColor.GetChannelColor(channel);
		}
	}
	
	public override string ToString() {
		if(playerName != "") {
			return "[" + channel + "] " + playerName + ": " + text;
		} else {
			if(channel != "")
				return "[" + channel + "] " + text;
			else
				return text;
		}
	}
}