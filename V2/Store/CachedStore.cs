using System.Collections.Generic;

namespace Futilef.V2.Store {
	public abstract class CachedStore<T> : IStore<T> {
		readonly Dictionary<string, T> dict = new Dictionary<string, T>();

		public T Get(string name) {
			T s;
			if (dict.TryGetValue(name, out s)) {
				return s;
			}
			s = Load(name);
			dict.Add(name, s);
			return s;
		}

		public abstract T Load(string name);
	}
}

