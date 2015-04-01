using NUnit.Framework;
using BoM.Async;

namespace BoM.Tests {
	// AsyncProperyTests
	class AsyncProperyTests : AsyncRequester {
		public const string famousPlayer = "Luke Skywalker";

		private AsyncProperty<string> playerName;

		[Test]
		public void Return_correct_value_after_waiting_50_ms() {
			playerName = new AsyncProperty<string>(this, "playerName");
			
			playerName.Get(data => {
				if(data == famousPlayer)
					Assert.Pass();
				else
					Assert.Fail();
			});
		}

		// RequestAsyncProperty
		public void RequestAsyncProperty(string propertyName) {
			if(propertyName == "playerName") {
				System.Threading.Thread.Sleep(50);
				playerName.directValue = famousPlayer;
			}
		}

		// WriteAsyncProperty
		public void WriteAsyncProperty(string propertyName, object val, WriteAsyncPropertyCallBack callBack) {
			// ...
		}
	}
}