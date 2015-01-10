using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class GenericSerializer {
	// Write a single value in JSON
	public static void WriteJSONValue(Jboy.JsonWriter writer, object val) {
		if(val is int || val is KeyCode) {
			writer.WriteNumber((double)((int)val));
		} else if(val is long) {
			writer.WriteNumber((double)((long)val));
		} else if(val is double) {
			writer.WriteNumber((double)val);
		} else {
			Jboy.Json.WriteObject(val, writer);
		}
	}
	
	// Writes all fields of a class instance
	public static void WriteJSON<T>(Jboy.JsonWriter writer, T instance, HashSet<string> fieldFilter = null, HashSet<string> fieldExclude = null) {
		if(instance == null) {
			writer.WriteNull();
			return;
		}
		
		// Type pointer
		Type type = typeof(T);
		
		// Obtain all fields
		FieldInfo[] fields = type.GetFields();
		
		// Loop through all fields
		writer.WriteObjectStart();
		
		foreach(var field in fields) {
			if(field.IsStatic)
				continue;
			
			// Get property name and value
			string name = field.Name;
			
			if(fieldFilter != null && !fieldFilter.Contains(name))
				continue;
			
			if(fieldExclude != null && fieldExclude.Contains(name))
				continue;
			
			object val = field.GetValue(instance);
			
			// Write them to the JSON stream
			writer.WritePropertyName(name);
			GenericSerializer.WriteJSONValue(writer, val);
		}
		
		writer.WriteObjectEnd();
	}

	// Read a single value from JSON
	public static object ReadJSONValue(Jboy.JsonReader reader, FieldInfo field) {
		var fieldType = field.FieldType;

		// Most common cases
		if(fieldType == typeof(int)) {
			return (int)(reader.ReadNumber());
		} else if(fieldType == typeof(uint)) {
			return (uint)(reader.ReadNumber());
		} else if(fieldType == typeof(long)) {
			return (long)(reader.ReadNumber());
		} else if(fieldType == typeof(byte)) {
			return (byte)(reader.ReadNumber());
		} else if(fieldType == typeof(double)) {
			return reader.ReadNumber();
		} else if(fieldType == typeof(float)) {
			return (float)reader.ReadNumber();
		} else if(fieldType == typeof(KeyCode)) {
			return (KeyCode)(reader.ReadNumber());
		} else if(fieldType == typeof(string)) {
			string stringObject;
			
			if(reader.TryReadString(out stringObject)) {
				return stringObject;
			} else {
				reader.ReadNull();
				return null;
			}
		}

		return ReadObject(fieldType, reader);
	}

	// ReadObject
	public static object ReadObject(Type fieldType, Jboy.JsonReader reader) {
		// Call via reflection: Jboy.Json.ReadObject<T>(reader)
		MethodInfo method = typeof(Jboy.Json).GetMethod("ReadObject", new [] { typeof(Jboy.JsonReader) });
		MethodInfo generic = method.MakeGenericMethod(fieldType);
		
		var parameters = new object[] {
			reader
		};
		
		return generic.Invoke(null, parameters);
	}

	// ReadObject
	public static object ReadObject(Type fieldType, string json) {
		// Call via reflection: Jboy.Json.ReadObject<T>(reader)
		MethodInfo method = typeof(Jboy.Json).GetMethod("ReadObject", new [] { typeof(string) });
		MethodInfo generic = method.MakeGenericMethod(fieldType);
		
		var parameters = new object[] {
			json
		};
		
		return generic.Invoke(null, parameters);
	}

	// Read a new class instance from JSON
	public static T ReadJSON<T>(Jboy.JsonReader reader) where T : new() {
		T instance = new T();
		
		reader.ReadObjectStart();
		
		string propName;
		bool success;
		var typeInfo = typeof(T);
		
		while(true) {
			success = reader.TryReadPropertyName(out propName);
			
			if(success) {
				var field = typeInfo.GetField(propName);
				
				if(field == null) {
					LogManager.DB.LogError("Field does not exist: '" + propName + "'");
					continue;
				}
				
				if(!field.IsStatic)
					field.SetValue(instance, GenericSerializer.ReadJSONValue(reader, field));
			} else {
				break;
			}
		}
		
		reader.ReadObjectEnd();
		
		return instance;
	}
}