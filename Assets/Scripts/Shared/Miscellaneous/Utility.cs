using System;

public static class Utility {
	// GetBuildDate
	public static DateTime GetBuildDate() {
		string filePath = System.Reflection.Assembly.GetCallingAssembly().Location;

		const int c_PeHeaderOffset = 60;
		const int c_LinkerTimestampOffset = 8;

		byte[] b = new byte[2048];
		System.IO.Stream s = null;
		
		try {
			s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
			s.Read(b, 0, 2048);
		} finally {
			if(s != null) {
				s.Close();
			}
		}
		
		int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
		int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);

		DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		dt = dt.AddSeconds(secondsSince1970);
		dt = dt.ToLocalTime();

		return dt;
	}
}
