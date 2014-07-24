using UnityEngine;
using uGameDB;
using uLobby;
using Jboy;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class GameDB : SingletonMonoBehaviour<GameDB> {
	public static bool codecsInitialized = false;
	public static Dictionary<string, string> accountIdToName = new Dictionary<string, string>();
	public static Dictionary<string, Guild> guildIdToGuild = new Dictionary<string, Guild>();
	public static Dictionary<string, List<GuildMember>> guildIdToGuildMembers = new Dictionary<string, List<GuildMember>>();
	public static List<List<RankEntry[]>> rankingLists;
	public static string logBucketPrefix = ""; //"<color=#ffcc00>";
	public static string logBucketMid = "["; //"</color>[<color=#00ffff>";
	public static string logBucketPostfix = "]"; //"</color>]";
	public static int maxEmailLength = 50;
	public static int maxPlayerNameLength = 25;
	public static int maxGuildNameLength = 30;
	public static int maxGuildTagLength = 4;
	public static int numRankingPages = 7;
	
	// Generic KeyValue map function
	public const string keyValueMapFunction =
		@"
		function(value, keydata, arg) {
			var parsedData = JSON.parse(value.values[0].data);
			
			var obj = new Object();
			obj.key = value.key;
			obj.val = parsedData;
			
			return [obj];
		}
		";
	
	// RegisterClassCodecs
	private static void RegisterClassCodecs(System.Type genericType) {
		foreach(var type in GetAllTypesImplementingGenericType(genericType)) {
			if(!type.IsGenericType) {
				var baseClassType = genericType.MakeGenericType(type);
				System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(baseClassType.TypeHandle);
			}
		}
	}
	
	// Initializes serialization codecs
	public static void InitCodecs() {
		// Prevent double call
		if(codecsInitialized)
			return;
		
		// Log
		LogManager.DB.Log("Initializing database codecs");
		
		// Register codecs automatically
		RegisterClassCodecs(typeof(JsonSerializable<>));
		
		// Force static constructor calls to register JSON MapReduce codecs
		new KeyValue<string>();
		new KeyValue<TimeStamp>();
		
		// Register JSON codec for player statistics
		Json.AddCodec<PlayerStats>(PlayerStats.JsonDeserializer, PlayerStats.JsonSerializer);
		Json.AddCodec<PlayerQueueStats>(PlayerQueueStats.JsonDeserializer, PlayerQueueStats.JsonSerializer);
		Json.AddCodec<InputControl>(InputControl.JsonDeserializer, InputControl.JsonSerializer);
		Json.AddCodec<InputSettings>(InputSettings.JsonDeserializer, InputSettings.JsonSerializer);
		Json.AddCodec<TimeStamp>(TimeStamp.JsonDeserializer, TimeStamp.JsonSerializer);
		Json.AddCodec<CharacterCustomization>(CharacterCustomization.JsonDeserializer, CharacterCustomization.JsonSerializer);
		
		// Register JSON codecs for Guilds
		Json.AddCodec<GuildMember>(GuildMember.JsonDeserializer, GuildMember.JsonSerializer);
		Json.AddCodec<GuildList>(GuildList.JsonDeserializer, GuildList.JsonSerializer);
		
		// Register JSON codecs for Inventory
		Json.AddCodec<Bag>(Bag.JsonDeserializer, Bag.JsonSerializer);
		Json.AddCodec<ItemSlot>(ItemSlot.JsonDeserializer, ItemSlot.JsonSerializer);
		Json.AddCodec<Item>(ItemSerializer.JsonDeserializer, ItemSerializer.JsonSerializer);
		Json.AddCodec<ItemInventory>(ItemInventory.JsonDeserializer, ItemInventory.JsonSerializer);

		// Register JSON codecs for Artifacts
		Json.AddCodec<Artifact>(Artifact.JsonDeserializer, Artifact.JsonSerializer);
		Json.AddCodec<ArtifactSlot>(ArtifactSlot.JsonDeserializer, ArtifactSlot.JsonSerializer);
		Json.AddCodec<ArtifactTree>(ArtifactTree.JsonDeserializer, ArtifactTree.JsonSerializer);
		Json.AddCodec<ArtifactInventory>(ArtifactInventory.JsonDeserializer, ArtifactInventory.JsonSerializer);

		// Register JSON codecs for MapReduce entries
		Json.AddCodec<RankEntry>(RankEntry.JsonDeserializer, RankEntry.JsonSerializer);
		
		// Register JSON codecs for integrated types
		Json.AddCodec<Color>(ColorSerializer.JsonDeserializer, ColorSerializer.JsonSerializer);
		Json.AddCodec<Texture2D>(Texture2DSerializer.JsonDeserializer, Texture2DSerializer.JsonSerializer);
		Json.AddCodec<ServerType>(ServerTypeSerializer.JsonDeserializer, ServerTypeSerializer.JsonSerializer);
		
		// BitStream codecs
		uLink.BitStreamCodec.AddAndMakeArray<RankEntry>(RankEntry.ReadFromBitStream, RankEntry.WriteToBitStream);
		uLink.BitStreamCodec.AddAndMakeArray<ChatMember>(ChatMember.ReadFromBitStream, ChatMember.WriteToBitStream);
		uLink.BitStreamCodec.AddAndMakeArray<GuildMember>(GuildMember.ReadFromBitStream, GuildMember.WriteToBitStream);
		uLink.BitStreamCodec.AddAndMakeArray<SkillBuild>(SkillBuild.ReadFromBitStream, SkillBuild.WriteToBitStream);
		uLink.BitStreamCodec.AddAndMakeArray<WeaponBuild>(WeaponBuild.ReadFromBitStream, WeaponBuild.WriteToBitStream);
		uLink.BitStreamCodec.AddAndMakeArray<AttunementBuild>(AttunementBuild.ReadFromBitStream, AttunementBuild.WriteToBitStream);
		uLink.BitStreamCodec.AddAndMakeArray<CharacterCustomization>(CharacterCustomization.ReadFromBitStream, CharacterCustomization.WriteToBitStream);
		uLink.BitStreamCodec.AddAndMakeArray<KeyValue<TimeStamp>>(KeyValue<TimeStamp>.ReadFromBitStream, KeyValue<TimeStamp>.WriteToBitStream);
		
		// Flag
		codecsInitialized = true;
	}
	
	// GetAllTypesImplementingGenericType
	public static IEnumerable<System.Type> GetAllTypesImplementingGenericType(System.Type genericType) {
		return
			genericType.Assembly.GetTypes().Where(
				t =>
					t.BaseType != null &&
					t.BaseType.IsGenericType &&
					t.BaseType.GetGenericTypeDefinition() == genericType
			);
	}
	
	// GetAllTypesUsingGenericType
	public static IEnumerable<System.Type> GetAllTypesUsingGenericType(System.Type genericType) {
		return
			genericType.Assembly.GetTypes().Where(
				t =>
					t.FullName.Contains("KeyValue")
					//t.IsGenericType &&
					//t.GetGenericTypeDefinition() == genericType
			);
	}
	
	// Initializes ranking lists
	public static void InitRankingLists() {
		// Subject -> Queue -> RankEntry
		rankingLists = new List<List<RankEntry[]>>();
		
		// Player
		rankingLists.Add(new List<RankEntry[]>());
		
		// Team
		rankingLists.Add(new List<RankEntry[]>());
		
		// Guild
		rankingLists.Add(new List<RankEntry[]>());
		
		// Fill with null
		foreach(var list in rankingLists) {
			for(byte i = 0; i < 7; i++) {
				list.Add(null);
			}
		}
	}
	
	// Delegate type
	public delegate void ActionOnResult<T>(T result);
	public delegate void PutActionOnResult<T>(string key, T result);
	
	// Resolve
	public static string Resolve(string key) {
		string name;
		if(GameDB.accountIdToName.TryGetValue(key, out name))
			return name; //+ " (" + key + ")";
		
		Guild guild;
		if(GameDB.guildIdToGuild.TryGetValue(key, out guild))
			return guild.name; //+ " (" + key + ")";
		
		return key;
	}
	
	// GetUniqueKey
	public static string GetUniqueKey() {
		return Hash(System.DateTime.UtcNow).ToString();
	}
	
	// Hash
	private static ulong Hash(System.DateTime when) {
		ulong kind = (ulong) (int) when.Kind;
		return (kind << 62) | (ulong) when.Ticks;
	}
	
	// IsTestAccount
	public static bool IsTestAccount(string email) {
		return email == "a" || email == "b" || email == "c" || email == "d" || email == "e";
	}
	
	// Static salt password
	public static string StaticSaltPassword(string password) {
		// Note that we can't use a dynamically generated salt here
		// because this is not the hash used in the actual database.
		// This is merely the hash stored in the registry and if we want
		// to provide the ability to log in via the website we'd need a way
		// for the website to access the registry to look up the salt.
		// Obviously that's not possible.
		return password + "c90e8eca04f64d70baacc9d0a5c4c72e" + password;
	}
	
	// Encrypt a password using SHA512
	public static byte[] EncryptPassword(string password) {
		// Make precalculated, generic rainbow tables ineffective by using a salt
		password = StaticSaltPassword(password);
		
		// Encrypt the password
		System.Security.Cryptography.SHA512 sha512 = System.Security.Cryptography.SHA512.Create();
		return sha512.ComputeHash(System.Text.Encoding.Unicode.GetBytes(password));
	}
	
	// Encrypt a password using SHA512
	public static string EncryptPasswordString(string password) {
		return System.Convert.ToBase64String(EncryptPassword(password));
	}
	
	// GetRandomString
	public static string GetRandomString(int lengthInBytes) {
		byte[] array = new byte[lengthInBytes];
		System.Security.Cryptography.RandomNumberGenerator.Create().GetBytes(array);
		return System.Convert.ToBase64String(array);
	}
	
	// Formats bucket name
	static string FormatBucketName(string bucketName) {
		var toIndex = bucketName.IndexOf("To");
		if(toIndex != -1) {
			return bucketName.Substring(toIndex + 2);
		}
		
		return bucketName;
	}
	
	// Format on success
	static string FormatSuccess(string key, string operation, string bucketName, object val) {
		if(operation != "get")
			return Resolve(key) + "." + operation + FormatBucketName(bucketName) + "(" + (val != null ? val.ToString() : "null") + ")";
		
		return Resolve(key) + "." + operation + FormatBucketName(bucketName) + "() -> " + (val != null ? val.ToString() : "null");
	}
	
	// Format on fail
	static string FormatFail(string key, string operation, string bucketName) {
		return Resolve(key) + "." + operation + FormatBucketName(bucketName) + "() FAIL";
	}
	
	// Get
	public static IEnumerator Get<T>(string bucketName, string key, ActionOnResult<T> func) {
		var stopWatch = Stopwatch.StartNew();
		
		var bucket = new Bucket(bucketName);
		var request = bucket.Get(key);
		yield return request.WaitUntilDone();
		
		stopWatch.Stop();
		
		if(request.isSuccessful) {
			T val = request.GetValue<T>();
			LogManager.DB.Log(FormatSuccess(key, "get", bucketName, val) + " (" + stopWatch.ElapsedMilliseconds + " ms)");
			func(val);
		} else {
			LogManager.DB.LogWarning(FormatFail(key, "get", bucketName) + " (" + stopWatch.ElapsedMilliseconds + " ms)");
			func(default(T));
		}
	}
	
	// Set
	public static IEnumerator Set<T>(string bucketName, string key, T val, ActionOnResult<T> func) {
		var stopWatch = Stopwatch.StartNew();
		
		var bucket = new Bucket(bucketName);
		var request = bucket.Set(key, val, Encoding.Json);
		yield return request.WaitUntilDone();
		
		stopWatch.Stop();
		
		if(request.isSuccessful) {
			LogManager.DB.Log(FormatSuccess(key, "set", bucketName, val) + " (" + stopWatch.ElapsedMilliseconds + " ms)");
			if(func != null)
				func(val);
		} else {
			LogManager.DB.LogWarning(FormatFail(key, "set", bucketName) + " (" + stopWatch.ElapsedMilliseconds + " ms)");
			if(func != null)
				func(default(T));
		}
	}
	
	// Put
	public static IEnumerator Put<T>(string bucketName, T val, PutActionOnResult<T> func) {
		var stopWatch = Stopwatch.StartNew();
		
		var bucket = new Bucket(bucketName);
		var request = bucket.SetGeneratedKey(val, Encoding.Json);
		yield return request.WaitUntilDone();
		
		stopWatch.Stop();
		
		if(request.isSuccessful) {
			string generatedKey = request.GetGeneratedKey();
			
			LogManager.DB.Log(FormatSuccess(generatedKey, "put", bucketName, val) + " (" + stopWatch.ElapsedMilliseconds + " ms)");
			func(generatedKey, val);
		} else {
			LogManager.DB.LogWarning(FormatFail("", "put", bucketName) + " (" + stopWatch.ElapsedMilliseconds + " ms)");
			func(default(string), default(T));
		}
	}
	
	// Remove
	public static IEnumerator Remove(string bucketName, string key, ActionOnResult<bool> func) {
		var stopWatch = Stopwatch.StartNew();
		
		var bucket = new Bucket(bucketName);
		var request = bucket.Remove(key);
		yield return request.WaitUntilDone();
		
		stopWatch.Stop();
		
		if(request.isSuccessful) {
			LogManager.DB.Log(FormatSuccess(key, "remove", bucketName, null) + " (" + stopWatch.ElapsedMilliseconds + " ms)");
			if(func != null)
				func(true);
		} else {
			LogManager.DB.LogWarning(FormatFail(key, "remove", bucketName) + " (" + stopWatch.ElapsedMilliseconds + " ms)");
			if(func != null)
				func(false);
		}
	}
	
	// MapReduce
	public static IEnumerator MapReduce<T>(string bucketName, string jsMapPhase, string jsReducePhase, object argument, ActionOnResult<T[]> func) {
		var stopWatch = Stopwatch.StartNew();
		
		var bucket = new Bucket(bucketName);
		var mapReduceRequest = bucket.MapReduce(
			new JavaScriptMapPhase(jsMapPhase),
			new JavaScriptReducePhase(jsReducePhase, argument)
		);
		
		// Wait until the request finishes
		yield return mapReduceRequest.WaitUntilDone();
		
		stopWatch.Stop();
		
		string logInfo = logBucketPrefix + bucketName + logBucketMid + argument.ToString() + logBucketPostfix;
		
		if(mapReduceRequest.isSuccessful) {
			var results = mapReduceRequest.GetResult<T>().ToArray();
			
			LogManager.DB.Log("MapReduce successful: " + logInfo + " -> " + typeof(T).ToString() + "[" + results.Length + "] (" + stopWatch.ElapsedMilliseconds + " ms)");
			func(results);
		} else {
			LogManager.DB.LogWarning("MapReduce failed: " + logInfo + " -> " + mapReduceRequest.GetErrorString() + " (" + stopWatch.ElapsedMilliseconds + " ms)");
			func(default(T[]));
		}
	}
	
	// --------------------------------------------------------------------------------
	// Generic MapReduce
	// --------------------------------------------------------------------------------
	
	// Search.Map
	public static string GetSearchMapFunction(string property) {
		return @"
			function(value, keydata, arg) {
				var obj = JSON.parse(value.values[0].data);
				
				var generatedObject = new Object();
				generatedObject.key = value.key;
				generatedObject.val = obj." + property + @";
				
				return [generatedObject];
			}
		";
	}
	
	// Search.Reduce
	public static string GetSearchReduceFunction(string condition = "obj.val == value") {
		return @"
			function(valueList, value) {
				var length = valueList.length;
				var obj = null;
				
				for(var i = 0; i < length; i++) {
					obj = valueList[i];
					if(" + condition + @")
						return [obj];
				}
				
				return [];
			}
		";
	}
	
	// SearchMultiple.Reduce
	public static string GetSearchMultipleReduceFunction(string condition = "obj.val == value") {
		return @"
			function(valueList, value) {
				var length = valueList.length;
				var obj = null;
				var result = [];
				
				for(var i = 0; i < length; i++) {
					obj = valueList[i];
					if(" + condition + @")
						result.push(obj);
				}
				
				return result;
			}
		";
	}
}
