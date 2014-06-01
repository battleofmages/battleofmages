using UnityEngine;
using System.Linq;

public static class StringExtensions {
	// Capitalize
	public static string Capitalize(this string input) {
		if(input.Length > 0)
			return System.Char.ToUpper(input[0]) + input.Substring(1);
		
		return input;
	}
	
	// ReplaceCommands
	public static string ReplaceCommands(this string input) {
		if(input.Length >= 2 && input.StartsWith("-") && !char.IsDigit(input[1]))
			return "//" + input.Substring(1);
		
		return input;
	}
	
	// PrettifyPlayerName
	public static string PrettifyPlayerName(this string playerNameRequest) {
		var lowered = playerNameRequest.Replace("  ", " ").ToLower();
		
		// Forbid repeated characters
		int repeatCount = 0;
		for(int i = 1; i < lowered.Length; i++) {
			if(lowered[i] == lowered[i - 1]) {
				if(repeatCount == 1) {
					// Remove character
					lowered = lowered.Remove(i);
					
					// Fix the iteration index
					i -= 1;
				} else {
					repeatCount++;
				}
			} else {
				repeatCount = 0;
			}
		}
		
		// Capitalize parts
		var parts = lowered.Split(' ');
		for(int i = 0; i < parts.Length; i++) {
			parts[i] = parts[i].Trim().Capitalize();
		}
		
		return string.Join(" ", parts);
	}

	// ToCleanGameObjectName
	public static string ToCleanGameObjectName(this string gameObjectName) {
		return gameObjectName.Replace("(Clone)", "").Replace("@Proxy", "").Replace("@Creator", "").Replace("@Owner", "");
	}
	
	// ContainsUnicodeCharacter
	public static bool ContainsUnicodeCharacter(this string input) {
		const int MaxAnsiCode = 255;
		
		return input.Any(c => c > MaxAnsiCode);
	}
	
	// HumanReadableInteger
	public static string HumanReadableInteger(this string number) {
		bool isNegative = false;
		
		if(number.StartsWith("-")) {
			isNegative = true;
			number = number.Substring(1);
		}
		
		if(number.Length < 4) {
			if(!isNegative)
				return number;
			else
				return "-" + number;
		}
		
		int partsCount = Mathf.CeilToInt((float)number.Length / 3);
		string[] parts = new string[partsCount];
		
		for(int i = 1; i <= partsCount; i++) {
			int pos = number.Length - i * 3;
			int length = 3;
			
			while(pos < 0) {
				pos++;
				length--;
			}
			
			parts[partsCount - i] = number.Substring(pos, length);
		}
		
		number = string.Join(System.Globalization.NumberFormatInfo.CurrentInfo.NumberGroupSeparator, parts);
		
		if(!isNegative)
			return number;
		else
			return "-" + number;
	}
}