using System.Text.RegularExpressions;

public class Validator {
	public static Regex accountName = new Regex(@"^\p{L}[\p{L}\p{N}]*$");
	public static Regex password = new Regex(@"^.{6,}$");
	public static Regex email = new Regex(@"^[\p{L}0-9!$'*+\-_]+(\.[\p{L}0-9!$'*+\-_]+)*@[\p{L}0-9\-]+(\.[\p{L}0-9]+)*(\.[\p{L}]{2,})$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
	public static Regex playerName = new Regex(@"^\p{L}[\p{L} ]{0," + (GameDB.maxPlayerNameLength - 2) + @"}\p{L}$");
	public static Regex guildName = new Regex(@"^[\p{Lu}][\p{L}\p{N} ]{0," + (GameDB.maxGuildNameLength - 2) + @"}[\p{L}\p{N}]$");
	public static Regex guildTag = new Regex(@"^[\p{L}\p{N}]{2," + GameDB.maxGuildTagLength + @"}$");
}