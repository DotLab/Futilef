namespace Futilef.V2 {
	public class ShaderStore : CachedStore<Shader> {
		public override Shader Load(string name) {
			return new Shader(name, UnityEngine.Shader.Find(name));
		}
	}
}

