namespace BoM.Async {
	// ConnectObjectCallBack
	public delegate void ConnectObjectCallBack(object val);

	// AsyncPropertyBase
	public class AsyncPropertyBase {
		// Get
		public virtual void GetObject(ConnectObjectCallBack callBack) {
			// ...
		}
	}
}