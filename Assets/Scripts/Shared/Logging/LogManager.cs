using System;
using System.Collections.Generic;

public static class LogManager {
	public static List<LogCategory> list = new List<LogCategory>();

	public static LogCategory General = null;
	public static LogCategory Online = null;
	public static LogCategory Chat = null;
	public static LogCategory DB = null;
	public static LogCategory System = null;
	public static LogCategory Spam = null;
#if !LOBBY_SERVER
	// Reserved
#endif
	
	// Static constructor
	static LogManager() {
		string logPath = DateTime.UtcNow.ToString("yyyy-MM-dd/HH-mm-ss/");
		
		// Initialize log path
#if UNITY_STANDALONE_WIN
		LogCategory.Init("./Logs/" + logPath);
#else
		LogCategory.Init("./logs/" + logPath);
#endif
		
		// Create logs
		LogManager.General = new LogCategory("General");
		LogManager.Chat = new LogCategory("Chat");
		LogManager.DB = new LogCategory("DB", false);
		LogManager.Online = new LogCategory("Online", false);
		LogManager.System = new LogCategory("System", false);
		LogManager.Spam = new LogCategory("Spam", false, false);
#if !LOBBY_SERVER
		// Reserved
#endif
	}
	
	// Close all
	public static void CloseAll() {
		foreach(var log in list) {
			log.Close();
		}
	}
}