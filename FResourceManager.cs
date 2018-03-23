using System;

using UnityEngine;

using Futilef.Serialization;

namespace Futilef {
	public sealed class FResource<T> : IDisposable where T : UnityEngine.Object {
		T _asset;
		public T asset {
			get {
				if (_asset != null) return _asset;
				throw new NullReferenceException();
			}
		}

		public FResource(string path) {
			_asset = Resources.Load<T>(path);
			if (_asset == null) throw new NullReferenceException();
		}

		public void Dispose() {
			Resources.UnloadAsset(_asset);
			_asset = null;
		}
	}

	public static class FResourceManager {
		public static BmFont LoadBmFont(string path) {
			using (var resource = new FResource<TextAsset>(path)) {
				using (var stream = new System.IO.MemoryStream(resource.asset.bytes)) {
					return new BmFont(stream);
				}
			}
		}

		public static TpAtlas LoadTpAtlas(string path) {
			using (var resource = new FResource<TextAsset>(path)) {
				return fastJSON.JSON.ToObject<TpAtlas>(resource.asset.text);
			}
		}

		public static Texture2D LoadTexture2D(string path) {
			using (var resource = new FResource<TextAsset>(path)) {
				var texture = new Texture2D(0, 0);
				texture.LoadImage(resource.asset.bytes);
				return texture;
			}
		}
	}
}