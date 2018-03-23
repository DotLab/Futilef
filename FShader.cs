using UnityEngine;

namespace Futilef {
	public sealed class FShader {
		static int _count = 0;

		public static readonly FShader Basic;
		public static readonly FShader Additive, AdditiveColor;

		static FShader() {
			Basic = new FShader("Basic", Shader.Find("Futile/Basic"));	
			Additive = new FShader("Additive", Shader.Find("Futile/Additive"));	
			AdditiveColor = new FShader("AdditiveColor", Shader.Find("Futile/AdditiveColor"));
		}

		public readonly int index;
		public readonly string name;

		public Shader shader;

		public FShader(string name, Shader shader) {
			index = _count++;
			this.name = name;
			this.shader = shader;
		}
	}
}

