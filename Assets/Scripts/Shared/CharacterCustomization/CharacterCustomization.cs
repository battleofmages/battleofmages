using UnityEngine;
using System.Collections;

[System.Serializable]
public class CharacterCustomization {
	public const float baseHeight = 1f;
	public const float baseVoicePitch = 1f;//1.12f;
	
	public const float heightStatMultiplier = 0.35f;
	public const float voicePitchMultiplier = 0.15f;//0.12f;
	
	public float height;
	public float voicePitch;
	
	public Color skinColor;
	public Color hairColor;
	public Color eyeColor;
	public Color eyeBackgroundColor;
	public Color cloakColor;
	public Color topWearColor;
	public Color legWearColor;
	public Color bootsColor;
	
	// Constructor
	public CharacterCustomization() {
		height = 0.5f;
		voicePitch = 0.5f;
		skinColor = Color.white;
		hairColor = Color.gray;
		eyeColor = Color.gray;
		eyeBackgroundColor = Color.white;
		cloakColor = new Color(0.3f, 0.3f, 0.3f, 1f);
		topWearColor = Color.gray;
		legWearColor = Color.gray;
		bootsColor = Color.gray;
	}
	
#if !LOBBY_SERVER
	// Update materials
	public void UpdateMaterials(Transform previewModel) {
		if(previewModel == null)
			return;
		
		var charDefinition = previewModel.GetComponent<CharacterDefinition>();
		
		var hairMaterial = charDefinition.hair.renderer.material;
		var eyesMaterial = charDefinition.eyes.renderer.material;
		var clothesMaterial = charDefinition.cloak.renderer.material;
		var topWearMaterial = charDefinition.topWear.renderer.material;
		var skirtMaterial = charDefinition.legWear.renderer.material;
		var stockingsMaterial = charDefinition.boots.renderer.material;
		
		hairMaterial.color = hairColor;
		eyesMaterial.color = eyeColor;
		eyesMaterial.SetColor("_BackgroundColor", eyeBackgroundColor);
		clothesMaterial.color = cloakColor;
		topWearMaterial.color = topWearColor;
		skirtMaterial.color = legWearColor;
		stockingsMaterial.color = bootsColor;
	}
	
	// Scale vector
	public Vector3 scaleVector {
		get {
			var newScale = heightMultiplier;
			return new Vector3(newScale, newScale, newScale);
		}
	}

	// Height multiplier
	public float heightMultiplier {
		get {
			return CharacterCustomization.baseHeight + (height - 0.5f) * CharacterCustomization.heightStatMultiplier;
		}
	}
	
	// Final voice pitch
	public float finalVoicePitch {
		get {
			return CharacterCustomization.baseVoicePitch - (voicePitch - 0.5f) * CharacterCustomization.voicePitchMultiplier;
		}
	}
#endif
	
	// Writer
	public static void JsonSerializer(Jboy.JsonWriter writer, object instance) {
		GenericSerializer.WriteJSONClassInstance<CharacterCustomization>(writer, (CharacterCustomization)instance);
	}
	
	// Reader
	public static object JsonDeserializer(Jboy.JsonReader reader) {
		return GenericSerializer.ReadJSONClassInstance<CharacterCustomization>(reader);
	}
	
	// BitStream Writer
	public static void WriteToBitStream(uLink.BitStream stream, object val, params object[] args) {
		var myObj = (CharacterCustomization)val;
		stream.Write<float>(myObj.height);
		stream.Write<float>(myObj.voicePitch);
		stream.Write<Color>(myObj.skinColor);
		stream.Write<Color>(myObj.hairColor);
		stream.Write<Color>(myObj.eyeColor);
		stream.Write<Color>(myObj.eyeBackgroundColor);
		stream.Write<Color>(myObj.cloakColor);
		stream.Write<Color>(myObj.topWearColor);
		stream.Write<Color>(myObj.legWearColor);
		stream.Write<Color>(myObj.bootsColor);
	}
	
	// BitStream Reader
	public static object ReadFromBitStream(uLink.BitStream stream, params object[] args) {
		var myObj = new CharacterCustomization();
		myObj.height = stream.Read<float>();
		myObj.voicePitch = stream.Read<float>();
		myObj.skinColor = stream.Read<Color>();
		myObj.hairColor = stream.Read<Color>();
		myObj.eyeColor = stream.Read<Color>();
		myObj.eyeBackgroundColor = stream.Read<Color>();
		myObj.cloakColor = stream.Read<Color>();
		myObj.topWearColor = stream.Read<Color>();
		myObj.legWearColor = stream.Read<Color>();
		myObj.bootsColor = stream.Read<Color>();
		return myObj;
	}
}
