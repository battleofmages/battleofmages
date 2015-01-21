using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Jboy;
using BoM;

public class Codecs : MonoBehaviour, Initializable {
	// Init
	public void Init() {
		LogManager.General.Log("Registering JSON codecs");

		// Register codecs automatically
		RegisterClassCodecs(typeof(JSONSerializable<>));

		// Force static constructor calls to register JSON MapReduce codecs
		new KeyValue<string>();
		new KeyValue<TimeStamp>();

		// Register JSON codecs for integrated types
		Json.AddCodec<Color>(ColorSerializer.ReadJSON, ColorSerializer.WriteJSON);

		// Enums
		RegisterEnumCodec<OnlineStatus>();
		RegisterEnumCodec<AddFriendError>();
		RegisterEnumCodec<RemoveFriendError>();
	}

	// RegisterEnumCodec
	void RegisterEnumCodec<T>() where T : System.IConvertible {
		Json.AddCodec<T>(EnumSerializer<T>.ReadJSON, EnumSerializer<T>.WriteJSON);
	}

	// RegisterClassCodecs
	private static void RegisterClassCodecs(System.Type genericType) {
		foreach(var type in GetAllTypesImplementingGenericType(genericType)) {
			if(!type.IsGenericType) {
				var baseClassType = genericType.MakeGenericType(type);
				System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(baseClassType.TypeHandle);
			}
		}
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
}
