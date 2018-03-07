using System;
using System.Collections.Generic;

using UnityEngine;

namespace Futilef {
	public class FAtlasElement {
		public int index;
		public string name;

		public FAtlas atlas;
		public int atlasIndex;

		public bool isTrimmed;

		public Rect uvRect;
		public Vector2 uvTopLeft;
		public Vector2 uvTopRight;
		public Vector2 uvBottomRight;
		public Vector2 uvBottomLeft;

		public Vector2 sourceSize;
		public Vector2 sourcePixelSize;
		public Rect sourceRect;
	}

	public class FAtlas {
		public readonly int index;
		public readonly string name, imagePath, dataPath;

		public readonly List<FAtlasElement> elements = new List<FAtlasElement>();
		public readonly Dictionary<string, FAtlasElement> elementMapping = new Dictionary<string, FAtlasElement>();

		public readonly Texture texture;
		public readonly Vector2 textureSize;

		public readonly bool _isSingleImage, _isTextureAnAsset;

		public FAtlas(int index, string name, Texture texture) {
			// Single atlas from texture
			this.index = index;
			this.name = name;
			this.texture = texture;
			textureSize = new Vector2(texture.width, texture.height);
			_isTextureAnAsset = false;

			CreateAtlasFromSingleImage();
			_isSingleImage = true;

		}

		public FAtlas(int index, string name, string imagePath, string dataPath = null) {
			// Single atlas from resource
			this.index = index;
			this.name = name;
			this.imagePath = imagePath;
			this.dataPath = dataPath;

			texture = Resources.Load<Texture>(imagePath);
			if (texture == null) throw new Exception("Cannot find resource");
			textureSize = new Vector2(texture.width, texture.height);
			_isTextureAnAsset = true;

			if (string.IsNullOrEmpty(dataPath)) CreateAtlasFromSingleImage();
			else CreateAtlasFromData();
			_isSingleImage = string.IsNullOrEmpty(dataPath);
		}

		void CreateAtlasFromSingleImage() {
			var element = new FAtlasElement();

			element.name = name;
			element.index = 0;

			element.atlas = this;
			element.atlasIndex = index;

			var uvRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

			element.uvRect = uvRect;

			element.uvTopLeft.Set(uvRect.xMin, uvRect.yMax);
			element.uvTopRight.Set(uvRect.xMax, uvRect.yMax);
			element.uvBottomRight.Set(uvRect.xMax, uvRect.yMin);
			element.uvBottomLeft.Set(uvRect.xMin, uvRect.yMin);

			element.sourceSize = new Vector2(textureSize.x / FScreen.ResourceScale, textureSize.y / FScreen.ResourceScale);
			element.sourcePixelSize = new Vector2(textureSize.x, textureSize.y);

			element.sourceRect = new Rect(0, 0, textureSize.x / FScreen.ResourceScale, textureSize.y / FScreen.ResourceScale);


			element.isTrimmed = false;

			elements.Add(element);
			elementMapping.Add(element.name, element);
		}

		void CreateAtlasFromData() {
			var dataAssert = Resources.Load<TextAsset>(dataPath);
			if (dataAssert == null) throw new Exception("Cannot load atlas data");

			var dict = dataAssert.text.dictionaryFromJson();
			var frames = (Dictionary<string, object>)dict["frames"];

			int i = 0;
			foreach (var item in frames) {
				var element = new FAtlasElement();

				element.index = i;
				element.name = Futilef.ShouldRemoveAtlasElementFileExtensions ? System.IO.Path.GetFileNameWithoutExtension(item.Key) : item.Key;

				element.atlas = this;
				element.atlasIndex = index;

				var itemDict = (Dictionary<string, object>)item.Value;
				element.isTrimmed = (bool)itemDict["trimmed"];
				if ((bool)itemDict["rotated"]) throw new Exception("Does not support rotated");

				var frame = (Dictionary<string, object>)itemDict["frame"];
				float frameX = float.Parse(frame["x"].ToString());
				float frameY = float.Parse(frame["y"].ToString());
				float frameW = float.Parse(frame["w"].ToString());
				float frameH = float.Parse(frame["h"].ToString()); 

				var uvRect = 
					new Rect(
						frameX / textureSize.x,
						((textureSize.y - frameY - frameH) / textureSize.y),
						frameW / textureSize.x,
						frameH / textureSize.y);

				element.uvRect = uvRect;

				element.uvTopLeft.Set(uvRect.xMin, uvRect.yMax);
				element.uvTopRight.Set(uvRect.xMax, uvRect.yMax);
				element.uvBottomRight.Set(uvRect.xMax, uvRect.yMin);
				element.uvBottomLeft.Set(uvRect.xMin, uvRect.yMin);

				var sourcePixelSize = (Dictionary<string, object>)itemDict["sourceSize"];
				element.sourcePixelSize.x = float.Parse(sourcePixelSize["w"].ToString());	
				element.sourcePixelSize.y = float.Parse(sourcePixelSize["h"].ToString());	

				element.sourceSize.x = element.sourcePixelSize.x / FScreen.ResourceScale;	
				element.sourceSize.y = element.sourcePixelSize.y / FScreen.ResourceScale;

				var sourceRect = (Dictionary<string, object>)itemDict["spriteSourceSize"];
				float rectX = float.Parse(sourceRect["x"].ToString()) / FScreen.ResourceScale;
				float rectY = float.Parse(sourceRect["y"].ToString()) / FScreen.ResourceScale;
				float rectW = float.Parse(sourceRect["w"].ToString()) / FScreen.ResourceScale;
				float rectH = float.Parse(sourceRect["h"].ToString()) / FScreen.ResourceScale;

				element.sourceRect = new Rect(rectX, rectY, rectW, rectH);

				elements.Add(element);
				elementMapping.Add(element.name, element);

				i += 1;
			}

			Resources.UnloadAsset(dataAssert);
		}
	}
}

