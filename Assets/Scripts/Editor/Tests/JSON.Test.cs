using NUnit.Framework;

namespace BoM {
	class JSONTest {
		[Test]
		public void StringTest() {
			var json = Jboy.Json.WriteObject("Hello World");
			Assert.AreEqual("Hello World", Jboy.Json.ReadObject<string>(json));
		}
	}
}