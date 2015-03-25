namespace BoM.Async {
	// WriteAsyncPropertyCallBack
	public delegate void WriteAsyncPropertyCallBack(object val);

	// AsyncRequester
	public interface AsyncRequester {
		void RequestAsyncProperty(string propertyName);
		void WriteAsyncProperty(string propertyName, object val, WriteAsyncPropertyCallBack callBack);
	}
}