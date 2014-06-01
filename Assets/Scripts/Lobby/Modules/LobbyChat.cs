using uLobby;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LobbyChat : LobbyModule<LobbyChat> {
	public string currentChannel;
	public int maxEntries = 50;
	public bool autoScroll = true;
	public GUIStyle textStyle;
	
	[HideInInspector]
	public List<ChatMessage> entries;
	
	[HideInInspector]
	public List<ChatMember> members;
	
	[HideInInspector]
	public bool chatInputEnabled = true;
	
	public bool chatInputFocused { get; protected set; }
	
	private Color chatInputColor;
	
	private List<string> myMessages;
	private int myMessagesIndex;
	
	private Vector2 chatScrollPosition;
	private string chatText = "";
	private InGameLobby inGameLobby;
	private string chatTextBeforeReturn;
	
	// Start
	void Start() {
		chatInputEnabled = true;
		entries = new List<ChatMessage>();
		members = new List<ChatMember>();
		myMessages = new List<string>();
		myMessages.Add("");
		
		inGameLobby = this.GetComponent<InGameLobby>();
		
#if UNITY_EDITOR
		if(!Application.CanStreamedLevelBeLoaded("Client")) {
			this.AddEntry("<size=32><color=red>WARNING: CLIENT SCENE DOES NOT EXIST!</color></size>");
		}
#endif
		
		// Listen to lobby events
		Lobby.AddListener(this);
	}
	
	// Draws the chat interface
	public override void Draw() {
		// Left side
		DrawChat();
		
		// Right side
		//DrawChatMembers();
	}
	
	// Draws the chat
	public void DrawChat() {
		using(new GUIVertical()) {
			// Chat messages
			using(new GUIScrollView(ref chatScrollPosition)) {
				using(new GUIVertical()) {
					for(int i = 0; i < entries.Count; i++) {
						var entry = entries[i];
						var msgRect = GUILayoutUtility.GetRect(entry.guiContent, textStyle);
						
						GUIHelper.Shadowed(
							msgRect.x, msgRect.y,
							(x, y) => {
								GUI.contentColor = entry.color;
								msgRect.x = x;
								msgRect.y = y;
								GUI.Label(msgRect, entry.guiContent, textStyle);
							}
						);
					}
					
					GUI.contentColor = Color.white;
				}
			}
			
			// Set chat input color
			if(inGameLobby.currentState == GameLobbyState.Game) {
				if(chatInputFocused) {
					chatInputColor = Color.white;
				} else {
					chatInputColor = new Color(1f, 1f, 1f, 0f);
				}
			} else {
				chatInputColor = Color.white;
			}
			
			GUI.color = chatInputColor;
			
			// Channel + input field
			using(new GUIHorizontal()) {
				// Channel switch
				if(inGameLobby.currentState != GameLobbyState.Game || chatInputFocused) {
					GUI.contentColor = GUIColor.GetChannelColor(currentChannel);
					GUILayout.Box(currentChannel, GUILayout.ExpandWidth(false));
					GUI.contentColor = Color.white;
				} else {
					GUILayout.Box("_", GUILayout.ExpandWidth(false));
				}
				
				// Save settings before changing them
				//var wordWrap = GUI.skin.textField.wordWrap;
				var richText = GUI.skin.textField.richText;
				//var fixedWidth = GUI.skin.textField.fixedWidth;
				
				//GUI.skin.textField.wordWrap = true;
				GUI.skin.textField.richText = true;
				//GUI.skin.textField.fixedWidth = GUIArea.width * 0.1f;
				
				// Chat input field
				GUI.enabled = chatInputEnabled && chatInputFocused;
				GUI.SetNextControlName("LobbyChatInput");
				GUILayout.Box("");
				
				chatText = GUI.TextField(GUILayoutUtility.GetLastRect(), chatText, 300);
				
				// Reset old settings
				//GUI.skin.textField.wordWrap = wordWrap;
				GUI.skin.textField.richText = richText;
				//GUI.skin.textField.fixedWidth = fixedWidth;
			}
			
			GUILayout.Space(4);
			
			GUI.enabled = true;
			GUI.color = Color.white;
			
			// Debug
			/*GUI.Label(new Rect(300, 0, 500, 25), GUI.GetNameOfFocusedControl());
			GUI.Label(new Rect(300, 25, 500, 25), Event.current.type.ToString());
			GUI.Label(new Rect(300, 50, 500, 25), inGameLobby.currentState.ToString());
			GUI.Label(new Rect(300, 75, 500, 25), chatInputEnabled.ToString());*/
			
			// Only on editing the current message we save it in array
			if(myMessagesIndex == myMessages.Count - 1) {
				myMessages[myMessages.Count - 1] = chatText;
			}
			
			if(chatInputFocused) {
				// This is for Asian IME input, we only send
				// if Return hasn't been used to change the text.
				if(Event.current.type == EventType.KeyDown) {
					chatTextBeforeReturn = chatText;
				}
				
				// The KeyUp event follows afterwards
				if(Event.current.type == EventType.KeyUp) {
					switch(Event.current.keyCode) {
						// Return
						case KeyCode.Return:
							if(chatText == chatTextBeforeReturn) {
								SendChat();
								
								if(inGameLobby.currentState == GameLobbyState.Game)
									GUIHelper.ClearAllFocus();
							}
							break;
						
						// Up
						case KeyCode.UpArrow:
							if(myMessages.Count == 0)
								break;
							
							myMessagesIndex -= 1;
							if(myMessagesIndex < 0) {
								myMessagesIndex = 0;
							}
							chatText = myMessages[myMessagesIndex];
							break;
						
						// Down
						case KeyCode.DownArrow:
							if(myMessages.Count == 0)
								break;
							
							myMessagesIndex += 1;
							if(myMessagesIndex >= myMessages.Count) {
								myMessagesIndex = myMessages.Count - 1;
								chatText = myMessages[myMessagesIndex];
							} else {
								chatText = myMessages[myMessagesIndex];
							}
							break;
						
						// Space
						case KeyCode.Space:
							ChannelChange();
							break;
						
						/*case KeyCode.A:
							if(Event.current.modifiers == EventModifiers.Control) {
								//GUIHelper.ClearAllFocus();
								//GUIHelper.Focus("LobbyChatInput");
								TextEditor t = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
								t.SelectAll();
								//LogManager.General.Log("Select all!");
							}
							break;*/
					}
				}
			} else {
				if(Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return && Login.instance.popupWindow == null) {
					GUIHelper.Focus("LobbyChatInput");
					Event.current.Use();
				}
			}
		}
		
		// Update focus info
		chatInputFocused = GUI.GetNameOfFocusedControl() == "LobbyChatInput";
	}
	
	// Change channel
	bool ChannelChange() {
		if(chatText.StartsWith("/")) {
			switch(chatText) {
				case "/g":
				case "/g ":
					currentChannel = "Global";
					chatText = "";
					return true;
					
				case "/a":
				case "/a ":
					if(PlayerAccount.mine.accessLevel >= AccessLevel.CommunityManager) {
						currentChannel = "Announcement";
						chatText = "";
						return true;
					}
					break;
					
				case "/m":
				case "/m ":
					currentChannel = "Map";
					chatText = "";
					return true;
			}
		}
		
		return false;
	}
	
	// Sends a chat message to the lobby server
	void SendChat() {
		if(chatText.Length > 0) {
			if(ChannelChange())
				return;
			
			myMessages[myMessages.Count - 1] = chatText;
			myMessages.Add("");
			myMessagesIndex = myMessages.Count - 1;
			
			if(Player.main == null || !Player.main.ProcessClientChatCommand(chatText)) {
				Lobby.RPC("ClientChat", Lobby.lobby, currentChannel, chatText);
			}
			
			chatText = "";
		}
	}
	
	// Adds a chat message
	public void AddEntry(string msg) {
		this.AddEntry("System", "", msg);
	}
	
	// Adds a chat message by a user on a specific channel
	public void AddEntry(string channel, string name, string msg) {
		if(entries.Count == maxEntries) {
			entries.RemoveAt(0);
		}
		
		ChatMessage chatMsg = new ChatMessage(
			channel,
			name,
			msg,
			new TimeStamp(System.DateTime.Now)
		);
		
		entries.Add(chatMsg);
		
		// TODO: Don't scroll if not at the bottom of the scroll view
		if(autoScroll)
			chatScrollPosition.y = float.MaxValue;
		
		LogManager.Chat.Log(chatMsg.ToString());
	}
	
	// Turn "Map@127.0.0.1:7000" into "Map"
	string FixChannelName(string channel) {
		var gameSepPos = channel.IndexOf('@');
		if(gameSepPos != -1)
			return channel.Substring(0, gameSepPos);
		
		return channel;
	}
	
	// --------------------------------------------------------------------------------
	// RPCs
	// --------------------------------------------------------------------------------
	
	[RPC]
	void Chat(string channel, string name, string msg) {
		channel = FixChannelName(channel);
		
		this.AddEntry(channel, name, msg);
	}
	
	[RPC]
	void ChatMembers(string channel, ChatMember[] members) {
		channel = FixChannelName(channel);
		//LogManager.General.Log("Received chat member list with " + members.Length + " entries");
		
		// TODO: Normally you should see the members for the currentChannel
		if(channel == "Global") {
			this.members = new List<ChatMember>(members);
			this.members = this.members.OrderBy(o => o.accountId).ToList();
		}
	}
	
	[RPC]
	void ChatJoin(string channel, ChatMember member, string playerName) {
		channel = FixChannelName(channel);
		//LogManager.General.Log("Chat member joined the channel: " + member.name);
		
		// TODO: Normally you should see the members for the currentChannel
		if(channel == "Global") {
			this.members.Add(member);
			this.members = this.members.OrderBy(o => o.accountId).ToList();
		}
		
		if(channel != "Announcement") {
			PlayerAccount.Get(member.accountId).playerName = playerName;
			this.AddEntry(channel, "", playerName + " joined the channel.");
		}
	}
	
	[RPC]
	void ChatLeave(string channel, ChatMember member) {
		channel = FixChannelName(channel);
		
		// TODO: Normally you should see the members for the currentChannel
		if(channel == "Global") {
			string accountId = member.accountId;
			string memberName = PlayerAccount.Get(accountId).playerName;
			int index = this.members.FindIndex(o => o.accountId == accountId);
			
			if(index != -1) {
				this.members.RemoveAt(index);
				this.AddEntry(channel, "", memberName + " left the channel.");
			}
		}
	}
	
	/*[RPC]
	void ChatStatus(string channel, ChatMember nMember) {
		var chatMember = members.Find(o => o.accountId == nMember.accountId);
		
		if(chatMember != null)
			chatMember.status = nMember.status;
	}*/
}
