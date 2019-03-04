using UnityEngine;

namespace Futilef.V2.Store {
	public class UnityResourceStore : IStore<byte[]> {
		public byte[] Get(string name) {
			return Resources.Load<TextAsset>(name).bytes;
		}
	}
}

