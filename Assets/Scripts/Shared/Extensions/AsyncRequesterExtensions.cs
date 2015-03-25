using BoM.Async;

static class AsyncRequesterExtensions {
	// GetProperty
	public static AsyncProperty<T> GetProperty<T>(this AsyncRequester obj, string propertyName) {
		return AsyncProperty<T>.GetProperty(obj, propertyName);
	}
}