using System;

namespace Futilef {
	public class Shader {
		public readonly int index;
		public readonly string name;

		public UnityEngine.Shader shader;

		public Shader(int index, string name) {
			this.index = index;
			this.name = name;
		}

		public virtual void SetProperties(UnityEngine.Material material) {
		}
	}
}

