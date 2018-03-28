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
		public static BmFont LoadBmFont(TpAtlas atlas, string path) {
			using (var resource = new FResource<TextAsset>(path)) {
				using (var stream = new System.IO.MemoryStream(resource.asset.bytes)) {
					var font = new BmFont(stream);
					font.atlas = atlas;

					foreach (var glyph in font.glyphDict.Values) {
						var frame = atlas.frameByName[font.pageNames[glyph.page]];
						frame.CalculateUvsInsideFrame(
							glyph.x, glyph.y, glyph.width, glyph.height,
							ref glyph.uvLeftBottom, ref glyph.uvLeftTop, ref glyph.uvRightTop, ref glyph.uvRightBottom);
					}

					return font;
				}
			}
		}

		public static TpAtlas LoadTpAtlas(Texture2D texture, string path) {
			using (var resource = new FResource<TextAsset>(path)) {
				var atlas = JsonUtility.FromJson<TpAtlas>(resource.asset.text);
				atlas.Init(texture);
				return atlas;
			}
		}

		public static Texture2D LoadTexture2D(string path, FilterMode mode = FilterMode.Bilinear) {
			using (var resource = new FResource<TextAsset>(path)) {
				var texture = new Texture2D(0, 0, TextureFormat.RGBAHalf, true);
				texture.filterMode = mode;
				texture.LoadImage(resource.asset.bytes);
				return texture;
			}
		}
	}
}