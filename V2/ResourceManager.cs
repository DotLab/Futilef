namespace Futilef.V2 {
	public class ResourceManager {
		public string defaultShaderName = "Default";

		public IStore<Shader> shaderStore;

		public IStore<TpDataFile> tpDataFileStore;
		public IStore<Texture> spriteTextureStore;

		public IStore<BmFontFile> bmFontFileStore;
		public IStore<Texture> fontTextureStore;

		public BmLabel CreateBmLabel(string fontName) {
			return new BmLabel(bmFontFileStore.Get(fontName), shaderStore.Get(defaultShaderName), fontTextureStore);
		}

		public TpSprite CreateTpSprite(string atlasName, string spriteName) {
			var file = tpDataFileStore.Get(atlasName);
			return new TpSprite(file, shaderStore.Get(defaultShaderName), spriteTextureStore.Get(file.name)).Sprite(spriteName);
		}

		public TpSpriteSliced CreateTpSpriteSliced(string atlasName, string spriteName) {
			var file = tpDataFileStore.Get(atlasName);
			return new TpSpriteSliced(file, shaderStore.Get(defaultShaderName), spriteTextureStore.Get(file.name)).Sprite(spriteName);
		}
	}
}

