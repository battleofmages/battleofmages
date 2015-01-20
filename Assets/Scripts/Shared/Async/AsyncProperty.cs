using System.Collections.Generic;

public class AsyncProperty<T> : AsyncPropertyBase {
	private T _val;
	private bool requestSent;
	private Dictionary<object, ConnectCallBack> keyToCallBack;

	// Constructor
	public AsyncProperty(AsyncRequester req, string propertyName) {
		name = propertyName;
		available = false;
		requestSent = false;
		_val = default(T);
		requester = req;
		keyToCallBack = new Dictionary<object, ConnectCallBack>();
	}

	// Delegate
	public delegate void ConnectCallBack(T val);

	// Event
	public event ConnectCallBack onValueChange;
	private event ConnectCallBack onReceive;

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
	public void Disconnect(object key) {
		ConnectCallBack callBack;

		if(!keyToCallBack.TryGetValue(key, out callBack))
			return;

		onValueChange -= callBack;
		
		if(!available)
			onReceive -= callBack;
	}

	// Disconnect
	public void Disconnect(ConnectCallBack callBack) {
		onValueChange -= callBack;

		if(!available)
			onReceive -= callBack;
	}

	// Get
	// One-time callback if you only care about the value once
	public void Get(ConnectCallBack callBack) {
		if(available) {
			callBack(_val);
			return;
		}

		onReceive += callBack;
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

	// Set
	public static void Set(object obj, string propertyName, T newVal) {
		var asyncProperty = (AsyncProperty<T>)obj.GetType().GetField(propertyName).GetValue(obj);
		asyncProperty.value = newVal;
	}

	// GetProperty
	public static AsyncProperty<T> GetProperty(object obj, string propertyName) {
		return (AsyncProperty<T>)obj.GetType().GetField(propertyName).GetValue(obj);
	}

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
			requester.WriteAsyncProperty(name, value, (newValue) => {
				_val = (T)newValue;
				
				if(available) {
					if(onValueChange != null)
						onValueChange(_val);
				} else {
					if(onReceive != null)
						onReceive(_val);

					available = true;
				}
			});
		}
	}
#endregion
}
