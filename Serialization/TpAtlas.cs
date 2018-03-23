using System.Collections.Generic;

using Dict = System.Collections.Generic.Dictionary<string, object>;

using UnityEngine;

namespace Futilef.Serialization {
	public class TpRect {
		public float x, y, w, h;

		public TpRect(Dict dict) {
			x = (long)dict["x"];
			y = (long)dict["y"];
			w = (long)dict["w"];
			h = (long)dict["h"];
		}

		public override string ToString() {
			return string.Format("[TpRect: x={0}, y={1}, w={2}, h={3}]", x, y, w, h);
		}
	}

	public class TpSize {
		public float w, h;

		public TpSize(Dict dict) {
			w = (long)dict["w"];
			h = (long)dict["h"];
		}

		public override string ToString() {
			return string.Format("[TpSize: w={0}, h={1}]", w, h);
		}
	}

	public class TpPosition {
		public float x, y;

		public TpPosition(Dict dict) {
			x = (float)(double)dict["x"];
			y = (float)(double)dict["y"];
		}

		public override string ToString() {
			return string.Format("[TpPosition: x={0}, y={1}]", x, y);
		}
	}

	public class TpMeta {
		public string app, version, image, format;
		public TpSize size;

		public TpMeta(Dict dict) {
			app = (string)dict["app"];
			version = (string)dict["version"];
			image = (string)dict["image"];
			format = (string)dict["format"];

			size = new TpSize((Dict)dict["size"]);
		}

		public override string ToString() {
			return string.Format("[TpMeta: app={0}, version={1}, image={2}, format={3}, size={4}]", app, version, image, format, size);
		}
	}

	public class TpFrame {
		public TpRect frame;
		public bool rotated, trimmed;
		public TpRect spriteSourceSize;
		public TpSize sourceSize;
		public TpPosition pivot;

		public TpFrame(Dict dict) {
			frame = new TpRect((Dict)dict["frame"]);
			rotated = (bool)dict["rotated"];
			trimmed = (bool)dict["trimmed"];
			spriteSourceSize = new TpRect((Dict)dict["spriteSourceSize"]);
			sourceSize = new TpSize((Dict)dict["sourceSize"]);
			pivot = new TpPosition((Dict)dict["pivot"]);

//			Debug.Log(this);
		}

		// Non-serialized
		public TpAtlas atlas;
		public Vector2 rectLeftBottom, rectLeftTop, rectRightTop, rectRightBottom;
		public Vector2 uvLeftBottom, uvLeftTop, uvRightTop, uvRightBottom;

		public void CalculateVertices(ref Vector2 rectLeftBottom, ref Vector2 rectLeftTop, ref Vector2 rectRightTop, ref Vector2 rectRightBottom) {
			float rectLeft = spriteSourceSize.x;
			float rectRight = spriteSourceSize.x + spriteSourceSize.w;
			float rectTop = sourceSize.h - (spriteSourceSize.y);
			float rectBottom = sourceSize.h - (spriteSourceSize.y + spriteSourceSize.h);

			float pivotX = sourceSize.w * pivot.x;
			float pivotY = sourceSize.h - sourceSize.h * pivot.y;

			rectLeftBottom.Set(rectLeft - pivotX, rectBottom - pivotY);
			rectLeftTop.Set(rectLeft - pivotX, rectTop - pivotY);
			rectRightTop.Set(rectRight - pivotX, rectTop - pivotY);
			rectRightBottom.Set(rectRight - pivotX, rectBottom - pivotY);		
		}

		public void CalculateVertices(float z, ref Vector3 rectLeftBottom, ref Vector3 rectLeftTop, ref Vector3 rectRightTop, ref Vector3 rectRightBottom) {
			float rectLeft = spriteSourceSize.x;
			float rectRight = spriteSourceSize.x + spriteSourceSize.w;
			float rectTop = sourceSize.h - (spriteSourceSize.y);
			float rectBottom = sourceSize.h - (spriteSourceSize.y + spriteSourceSize.h);

			float pivotX = sourceSize.w * pivot.x;
			float pivotY = sourceSize.h - sourceSize.h * pivot.y;

			rectLeftBottom.Set(rectLeft - pivotX, rectBottom - pivotY, z);
			rectLeftTop.Set(rectLeft - pivotX, rectTop - pivotY, z);
			rectRightTop.Set(rectRight - pivotX, rectTop - pivotY, z);
			rectRightBottom.Set(rectRight - pivotX, rectBottom - pivotY, z);		
		}

		public void CalculateUvsInsideFrame(
			float glyphX, float glyphY, float glyphW, float glyphH,
			ref Vector2 uvLeftBottom, ref Vector2 uvLeftTop, ref Vector2 uvRightTop, ref Vector2 uvRightBottom) {

			float frameX = rotated ? (frame.x + frame.h - glyphY - glyphH) : (frame.x + glyphX);
			float frameY = rotated ? (frame.y + glyphX) : (frame.y + glyphY);
			float frameW = glyphW;
			float frameH = glyphH;

			atlas.CalculateUvsInsideAtlas(
				rotated, frameX, frameY, frameW, frameH, 
				ref uvLeftBottom, ref uvLeftTop, ref uvRightTop, ref uvRightBottom);
		}

		public override string ToString() {
			return string.Format("[TpFrame: frame={0}, rotated={1}, trimmed={2}, spriteSourceSize={3}, sourceSize={4}, pivot={5}]", frame, rotated, trimmed, spriteSourceSize, sourceSize, pivot);
		}
	}

	public class TpAtlas {
		public Dictionary<string, TpFrame> frames = new Dictionary<string, TpFrame>();
		public TpMeta meta;

		public TpAtlas(Dict dict) {
			var framesDict = (Dict)dict["frames"];
			foreach (var pair in framesDict) {
				Debug.Log(pair.Value.GetType());
				frames.Add(pair.Key, new TpFrame((Dict)pair.Value));
			}

			meta = new TpMeta((Dict)dict["meta"]);
		}

		// Non-serialized
		public Texture2D texture;

		public void CalculateUvsInsideAtlas(
			bool rotated, float frameX, float frameY, float frameW, float frameH, 
			ref Vector2 uvLeftBottom, ref Vector2 uvLeftTop, ref Vector2 uvRightTop, ref Vector2 uvRightBottom) {

			float width = meta.size.w;
			float height = meta.size.h;

			if (!rotated) {
				float uvLeft = frameX / width;
				float uvRight = (frameX + frameW) / width;
				float uvTop = 1f - frameY / height;
				float uvBottom = 1f - (frameY + frameH) / height;

				uvLeftBottom.Set(uvLeft, uvBottom);
				uvLeftTop.Set(uvLeft, uvTop);	
				uvRightTop.Set(uvRight, uvTop);
				uvRightBottom.Set(uvRight, uvBottom);
			} else {  // Is rotated
				float uvLeft = frameX / width;
				float uvRight = (frameX + frameH) / width;
				float uvTop = 1f - frameY / height;
				float uvBottom = 1f - (frameY + frameW) / height;

				uvLeftBottom.Set(uvLeft, uvTop);	
				uvLeftTop.Set(uvRight, uvTop);
				uvRightTop.Set(uvRight, uvBottom);
				uvRightBottom.Set(uvLeft, uvBottom);
			}
		}

		public override string ToString() {
			return string.Format("[TpAtlas: frames={0}, meta={1}]", frames, meta);
		}
	}
}

