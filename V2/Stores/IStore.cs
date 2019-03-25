using System.Collections.Generic;

namespace Futilef.V2 {
	public interface IStore<T> {
		T Get(string name);
	}

	public sealed class ScopedStore <T> : IStore<T> {
		public readonly string scope;
		public readonly IStore<T> store;

		public ScopedStore(string scope, IStore<T> store) {
			this.scope = scope;
			this.store = store;
		}
		
		public T Get(string name) {
			return store.Get(scope + name);
		}
	}

	public sealed class CachedStore<T> : IStore<T> {
		public readonly IStore<T> store;

		readonly Dictionary<string, T> dict = new Dictionary<string, T>();

		public CachedStore(IStore<T> store) {
			this.store = store;
		}

		public T Get(string name) {
			T s;
			if (dict.TryGetValue(name, out s)) {
				return s;
			}
			s = store.Get(name);
			dict.Add(name, s);
			return s;
		}
	}

	public sealed class ShaderStore : IStore<Shader> {
		public Shader Get(string name) {
			return new Shader(name, UnityEngine.Shader.Find(name));
		}
	}

	public sealed class TextureStore : IStore<Texture> {
		public readonly IStore<byte[]> store;
		public readonly bool transform8BitPng;

		public TextureStore(IStore<byte[]> store) {
			this.store = store;
		}

		public TextureStore(IStore<byte[]> store, bool transform8BitPng) {
			this.store = store;
			this.transform8BitPng = transform8BitPng;
		}

		public Texture Get(string name) {
			var bytes = store.Get(name);
			var unityTexture = new UnityEngine.Texture2D(2, 2);
			UnityEngine.ImageConversion.LoadImage(unityTexture, bytes);

			var t = new Texture(name, unityTexture);
			if (transform8BitPng) t.Transform8BitPng();
			return t;
		}
	}

	public sealed class UnityResourceByteStore : IStore<byte[]> {
		public byte[] Get(string name) {
			return UnityEngine.Resources.Load<UnityEngine.TextAsset>(name).bytes;
		}
	}

	public sealed class UnityResourceTextStore : IStore<string> {
		public string Get(string name) {
			return UnityEngine.Resources.Load<UnityEngine.TextAsset>(name).text;
		}
	}

	public sealed class BmFontFileStore : IStore<BmFontFile> {
		public readonly IStore<byte[]> store;

		public BmFontFileStore(IStore<byte[]> store) {
			this.store = store;
		}
		
		public BmFontFile Get(string name) {
			return new BmFontFile(store.Get(name));
		}
	}

	public sealed class TpDataFileStore : IStore<TpDataFile> {
		public readonly IStore<string> store;

		public TpDataFileStore(IStore<string> store) {
			this.store = store;
		}
		
		public TpDataFile Get(string name) {
			return new TpDataFile(store.Get(name));
		}
	}
}
