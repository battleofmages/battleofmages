using System.Collections.Generic;

namespace BoM.Async {
	// AsyncProperty
	public class AsyncProperty<T> : AsyncPropertyBase {
		private T _val;
		private bool requestSent;
		private Dictionary<object, ConnectCallBack> keyToCallBack;

		// Delegate
		public delegate void ConnectCallBack(T val);
		
		// Event
		public event ConnectCallBack onValueChange;
		private event ConnectCallBack onReceive;

		// Constructor
		public AsyncProperty(AsyncRequester req, string propertyName) {
			name = propertyName;
			requester = req;
			keyToCallBack = new Dictionary<object, ConnectCallBack>();
			Clear();
		}

		// Connect
		// Permanently connects the value change to the event
		public void Connect(object key, ConnectCallBack callBack, bool request = true) {
			if(key != null)
				keyToCallBack[key] = callBack;

			onValueChange += callBack;

			if(available) {
				callBack(_val);
				return;
			} else {
				onReceive += callBack;
			}

			if(request)
				Request();
		}

		// Connect
		public void Connect(ConnectCallBack callBack, bool request = true) {
			Connect(null, callBack, request);
		}

		// Disconnect
		public bool Disconnect(object key) {
			ConnectCallBack callBack;

			if(!keyToCallBack.TryGetValue(key, out callBack))
				return false;
			
			onValueChange -= callBack;
			onReceive -= callBack;

			return true;
		}

		// Disconnect
		public void Disconnect(ConnectCallBack callBack) {
			onValueChange -= callBack;
			onReceive -= callBack;
		}

		// Get
		// One-time callback if you only care about the value once
		public void Get(ConnectCallBack callBack, bool request = true) {
			if(available) {
				callBack(_val);
				return;
			}

			onReceive += callBack;

			if(request)
				Request();
		}

		// GetObject
		public override void GetObject(ConnectObjectCallBack callBack) {
			if(available) {
				callBack(_val);
				return;
			}
			
			onReceive += (T newVal) => {
				callBack(newVal);
			};

			Request();
		}

		// Clear
		public void Clear() {
			available = false;
			requestSent = false;
			_val = default(T);
		}

		// Request
		public void Request() {
			if(requestSent)
				return;
			
			Update();
			requestSent = true;
		}

		// Update
		public void Update() {
			requester.RequestAsyncProperty(name);
		}

	#region Static
		// Set
		public static void Set(object obj, string propertyName, T newVal) {
			var asyncProperty = (AsyncProperty<T>)obj.GetType().GetField(propertyName).GetValue(obj);
			asyncProperty.value = newVal;
		}

		// GetProperty
		public static AsyncProperty<T> GetProperty(object obj, string propertyName) {
			return (AsyncProperty<T>)obj.GetType().GetField(propertyName).GetValue(obj);
		}
	#endregion

	#region Properties
		// Available
		public bool available {
			get;
			protected set;
		}

		// Name
		public string name {
			get;
			protected set;
		}

		public AsyncRequester requester {
			get;
			protected set;
		}

		// Value
		public T value {
			get {
				return _val;
			}

			set {
				requester.WriteAsyncProperty(name, value, ignored => {
					directValue = value;
				});
			}
		}

		// Value
		public T directValue {
			set {
				_val = value;
				
				if(available) {
					if(onValueChange != null)
						onValueChange(_val);
				} else {
					available = true;

					if(onReceive != null)
						onReceive(_val);
				}
			}
		}
	#endregion
	}
}