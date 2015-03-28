using uGameDB;
using System.Collections;
using System.Linq;
using System.Diagnostics;

public class GameDB : SingletonMonoBehaviour<GameDB> {
	public static int maxEmailLength = 50;
	public static int maxPlayerNameLength = 25;
	public static int maxGuildNameLength = 30;
	public static int maxGuildTagLength = 4;

	// Delegate types
	public delegate void ActionOnResult<T>(T result);
	public delegate void PutActionOnResult<T>(string key, T result);

#region Queries
	// Get
	public static IEnumerator Get<T>(string bucketName, string key, ActionOnResult<T> func) {
		var stopWatch = Stopwatch.StartNew();
		
		var bucket = new Bucket(bucketName);
		var request = bucket.Get(key);
		yield return request.WaitUntilDone();
		
		stopWatch.Stop();
		
		if(request.isSuccessful) {
			T val = request.GetValue<T>();
			LogManager.DB.Log(GameDBFormatter.Success(key, "get", bucketName, val) + " (" + stopWatch.ElapsedMilliseconds + " ms)");
			func(val);
		} else {
			LogManager.DB.LogWarning(GameDBFormatter.Fail(key, "get", bucketName) + " (" + stopWatch.ElapsedMilliseconds + " ms)");
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
			LogManager.DB.Log(GameDBFormatter.Success(key, "set", bucketName, val) + " (" + stopWatch.ElapsedMilliseconds + " ms)");
			if(func != null)
				func(val);
		} else {
			LogManager.DB.LogWarning(GameDBFormatter.Fail(key, "set", bucketName) + " (" + stopWatch.ElapsedMilliseconds + " ms)");
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
			
			LogManager.DB.Log(GameDBFormatter.Success(generatedKey, "put", bucketName, val) + " (" + stopWatch.ElapsedMilliseconds + " ms)");
			func(generatedKey, val);
		} else {
			LogManager.DB.LogWarning(GameDBFormatter.Fail("", "put", bucketName) + " (" + stopWatch.ElapsedMilliseconds + " ms)");
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
			LogManager.DB.Log(GameDBFormatter.Success(key, "remove", bucketName, null) + " (" + stopWatch.ElapsedMilliseconds + " ms)");
			if(func != null)
				func(true);
		} else {
			LogManager.DB.LogWarning(GameDBFormatter.Fail(key, "remove", bucketName) + " (" + stopWatch.ElapsedMilliseconds + " ms)");
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
		
		string logInfo = bucketName + "[" + argument.ToString() + "]";
		
		if(mapReduceRequest.isSuccessful) {
			var results = mapReduceRequest.GetResult<T>().ToArray();
			
			LogManager.DB.Log("MapReduce successful: " + logInfo + " -> " + typeof(T).ToString() + "[" + results.Length + "] (" + stopWatch.ElapsedMilliseconds + " ms)");
			func(results);
		} else {
			LogManager.DB.LogWarning("MapReduce failed: " + logInfo + " -> " + mapReduceRequest.GetErrorString() + " (" + stopWatch.ElapsedMilliseconds + " ms)");
			func(default(T[]));
		}
	}
#endregion

#region Utilities
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

	// MD5
	public static string MD5(string password) {
		// Byte array representation of that string
		byte[] encodedPassword = new System.Text.UTF8Encoding().GetBytes(password);

		// Need MD5 to calculate the hash
		byte[] hash = ((System.Security.Cryptography.HashAlgorithm) System.Security.Cryptography.CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
		
		// String representation (similar to UNIX format)
		return System.BitConverter.ToString(hash)
			// Without dashes
			.Replace("-", string.Empty)
				// Make lowercase
				.ToLower();
	}
#endregion
	
#region Common MapReduce functions
	// Search.Map
	public static string GetSearchMapFunction(string property) {
		return @"
			function(value, keydata, arg) {
				var obj = JSON.parse(value.values[0].data);
				
				return [{
					key: value.key,
					val: obj." + property + @"
				}];
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
#endregion
}