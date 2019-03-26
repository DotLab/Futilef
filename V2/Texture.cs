namespace Futilef.V2 {
	public class Texture {
		public readonly string name;
		public readonly UnityEngine.Texture2D unityTexture;

		public Texture(string name, UnityEngine.Texture2D unityTexture) {
			this.name = name;
			this.unityTexture = unityTexture;
		}
	}
}

