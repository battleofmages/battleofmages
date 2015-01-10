public delegate void WriteAsyncPropertyCallBack(object val);

public interface AsyncRequester {
	void RequestAsyncProperty(string propertyName);
	void WriteAsyncProperty(string propertyName, object val, WriteAsyncPropertyCallBack callBack);
}