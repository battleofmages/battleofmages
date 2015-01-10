using UnityEngine;
using System.Collections;

public static class GameDBFormatter {
	// Resolve
	public static string Resolve(string key) {
		return key;
	}

	// Format on success
	public static string Success(string key, string operation, string bucketName, object val) {
		if(operation != "get")
			return Resolve(key) + "." + operation + BucketName(bucketName) + "(" + (val != null ? val.ToString() : "null") + ")";
		
		return Resolve(key) + "." + operation + BucketName(bucketName) + "() -> " + (val != null ? val.ToString() : "null");
	}
	
	// Format on fail
	public static string Fail(string key, string operation, string bucketName) {
		return Resolve(key) + "." + operation + BucketName(bucketName) + "() FAIL";
	}

	// Formats bucket name
	public static string BucketName(string bucketName) {
		var toIndex = bucketName.IndexOf("To");
		if(toIndex != -1) {
			return bucketName.Substring(toIndex + 2);
		}
		
		return bucketName;
	}
}
