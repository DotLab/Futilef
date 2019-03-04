namespace Futilef.V2 {
	public sealed class Shader {
		public readonly string name;
		public readonly UnityEngine.Shader unityShader;

		public Shader(string name, UnityEngine.Shader unityShader) {
			this.name = name;
			this.unityShader = unityShader;
		}
	}
}
