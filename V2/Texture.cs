namespace Futilef.V2 {
	public class Texture {
		public readonly string name;
		public readonly UnityEngine.Texture2D unityTexture;

		public Texture(string name, UnityEngine.Texture2D unityTexture) {
			this.name = name;
			this.unityTexture = unityTexture;
			unityTexture.wrapMode = UnityEngine.TextureWrapMode.Clamp;
		}

		public void Transform8BitPng() {
			var pixels = unityTexture.GetPixels32();
			for (int i = 0, len = pixels.Length; i < len; i += 1) {
				pixels[i].a = pixels[i].r;
				pixels[i].r = 255;
				pixels[i].g = 255;
				pixels[i].b = 255;
			}
			unityTexture.SetPixels32(pixels);
			unityTexture.Apply();
		}
	}
}

