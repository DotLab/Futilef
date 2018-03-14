using System;

namespace Futilef {
	public class Atlas {
		public readonly int index;
		public readonly string name;

		public UnityEngine.Texture2D texture;

		public Atlas(int index, string name) {
			this.index = index;
			this.name = name;
		}
	}
}

