namespace Futilef.V2.Store {
	public interface IStore<T> {
		T Get(string name);
	}
}
