using System.Collections.Generic;

namespace Futilef.V2.Store {
	public class TextureStore : CachedStore<Texture> {
		public IStore<byte[]> provider;

		public TextureStore(IStore<byte[]> provider) {
			this.provider = provider;
		}

		public override Texture Load(string name) {
			var bytes = provider.Get(name);
			var unityTexture = new UnityEngine.Texture2D(2, 2);
			UnityEngine.ImageConversion.LoadImage(unityTexture, bytes);
			return new Texture(name, unityTexture);
		}
	}
}

