public class AsyncPropertyBase {
	// Delegate
	public delegate void ConnectObjectCallBack(object val);
	
	// Get
	public virtual void GetObject(ConnectObjectCallBack callBack) {
		
	}
}