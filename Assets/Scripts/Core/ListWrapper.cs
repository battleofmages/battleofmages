using System;
using System.Collections.Generic;

namespace BoM.Core {
	[Serializable]
	public class ListWrapper<T> {
		public List<T> list;

		public IEnumerator<T> GetEnumerator() {
			return list.GetEnumerator();
		}
	}
}
