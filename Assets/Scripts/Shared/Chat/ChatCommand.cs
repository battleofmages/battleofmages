using System.Text.RegularExpressions;

public class ChatCommand<T> {
	// RegExCallback
	public delegate void ChatCommandCallback(T player, string[] args);
	
	// Constructor
	public ChatCommand(string regExString, ChatCommandCallback nCallBack) {
		regEx = new Regex(regExString, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
		callBack = nCallBack;
	}
	
	// Process
	public bool Process(T player, string cmd) {
		var match = regEx.Match(cmd);
		
		// No command has been executed
		if(!match.Success)
			return false;
		
		// Create arguments array
		string[] args = new string[match.Groups.Count - 1];
		for(int i = 0; i < args.Length; i++)
			args[i] = match.Groups[i + 1].Value;
		
		// Execute command
		callBack(player, args);
		
		// A command has been executed
		return true;
	}
	
	// Regex
	public Regex regEx {
		get;
		protected set;
	}
	
	// Callback
	public ChatCommandCallback callBack {
		get;
		protected set;
	}
}