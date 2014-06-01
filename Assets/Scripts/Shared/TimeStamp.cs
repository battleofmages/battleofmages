using System.Collections.Generic;

[System.Serializable]
public class TimeStamp {
	public double unixTimeStamp;
	
	// Constructor
	public TimeStamp() {
		unixTimeStamp = DateTimeToUnixTimeStamp(System.DateTime.UtcNow);
	}
	
	// Constructor
	public TimeStamp(System.DateTime dt) {
		unixTimeStamp = DateTimeToUnixTimeStamp(dt);
	}
	
	// Readable datetime
	public string readableDateTime {
		get {
			var dateTime = UnixTimeStampToDateTime(unixTimeStamp);
			return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
		}
	}
	
	// Datetime object
	public System.DateTime dateTime {
		get {
			var dateTime = UnixTimeStampToDateTime(unixTimeStamp);
			return dateTime;
		}
	}
	
	// Writer
	public static void JsonSerializer(Jboy.JsonWriter writer, object instance) {
		var fieldFilter = new HashSet<string>() {
			"unixTimeStamp",
		};
		GenericSerializer.WriteJSONClassInstance<TimeStamp>(writer, (TimeStamp)instance, fieldFilter);
	}
	
	// Reader
	public static object JsonDeserializer(Jboy.JsonReader reader) {
		return GenericSerializer.ReadJSONClassInstance<TimeStamp>(reader);
	}
	
	// Unix Timestamp -> DateTime
	public static System.DateTime UnixTimeStampToDateTime(double nUnixTimeStamp) {
		// Unix timestamp is seconds past epoch
		System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
		dtDateTime = dtDateTime.AddSeconds(nUnixTimeStamp).ToUniversalTime();
		return dtDateTime;
	}
	
	// DateTime -> Unix Timestamp
	public static double DateTimeToUnixTimeStamp(System.DateTime date) {
		System.TimeSpan span = (date - new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc));
		return span.TotalSeconds;
	}
	
	// BitStream Writer
	public static void WriteToBitStream(uLink.BitStream stream, object val, params object[] args) {
		var obj = (TimeStamp)val;
		
		stream.WriteDouble(obj.unixTimeStamp);
	}
	
	// BitStream Reader
	public static object ReadFromBitStream(uLink.BitStream stream, params object[] args) {
		var obj = new TimeStamp();
		
		obj.unixTimeStamp = stream.ReadDouble();
		
		return obj;
	}
	
	// ToString
	public override string ToString() {
		return readableDateTime;
	}
}
