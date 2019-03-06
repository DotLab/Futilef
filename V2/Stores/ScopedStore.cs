namespace Futilef.V2 {
	public sealed class ScopedStore <T> : IStore<T> {
		public readonly IStore<T> store;
		public readonly string scope;

		public ScopedStore(IStore<T> store, string scope) {
			this.store = store;
			this.scope = scope;
		}

		public T Get(string name) {
			return store.Get(scope + name);
		}
	}
}

