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
		public bool setWrapModeClamp = true;
		public bool setFilterModePoint;
		public bool transform8BitPng;

		public TextureStore(IStore<byte[]> store) {
			this.store = store;
		}

		public TextureStore(IStore<byte[]> store, bool transform8BitPng) {
			this.store = store;
			this.transform8BitPng = transform8BitPng;
		}

		public Texture Get(string name) {
			var bytes = store.Get(name);
			var unityTexture = new UnityEngine.Texture2D(0, 0);
			UnityEngine.ImageConversion.LoadImage(unityTexture, bytes);

			if (setWrapModeClamp) unityTexture.wrapMode = UnityEngine.TextureWrapMode.Clamp;
			if (setFilterModePoint) unityTexture.filterMode = UnityEngine.FilterMode.Point;
			if (transform8BitPng) {
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

			return new Texture(name, unityTexture);
		}
	}

	public sealed class UnityResourceByteStore : IStore<byte[]> {
		public byte[] Get(string name) {
//			UnityEngine.Debug.LogFormat("Load Bytes {0}", name);
			return UnityEngine.Resources.Load<UnityEngine.TextAsset>(name).bytes;
		}
	}

	public sealed class UnityResourceTextStore : IStore<string> {
		public string Get(string name) {
//			UnityEngine.Debug.LogFormat("Load Text {0}", name);
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
