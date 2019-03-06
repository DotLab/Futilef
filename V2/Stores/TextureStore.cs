namespace Futilef.V2 {
	public class TextureStore : CachedStore<Texture> {
		public bool shouldTransform8BitPng;
		public IStore<byte[]> provider;

		public TextureStore(IStore<byte[]> provider) {
			this.provider = provider;
		}

		public override Texture Load(string name) {
			var bytes = provider.Get(name);
			var unityTexture = new UnityEngine.Texture2D(2, 2);
			UnityEngine.ImageConversion.LoadImage(unityTexture, bytes);

			var t = new Texture(name, unityTexture);
			if (shouldTransform8BitPng) t.Transform8BitPng();
			return t;
		}
	}
}

