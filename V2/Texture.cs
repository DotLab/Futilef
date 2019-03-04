namespace Futilef.V2 {
	public class Texture {
		public readonly string name;
		public readonly UnityEngine.Texture unityTexture;

		public Texture(string name, UnityEngine.Texture unityTexture) {
			this.name = name;
			this.unityTexture = unityTexture;
			unityTexture.wrapMode = UnityEngine.TextureWrapMode.Clamp;
		}
	}
}

