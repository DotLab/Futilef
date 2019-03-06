namespace Futilef.V2 {
	public interface IStore<T> {
		T Get(string name);
	}
}
