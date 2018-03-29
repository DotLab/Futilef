using System;
using System.Collections.Generic;

using UnityEngine;

namespace Futilef.Serialization {
	[Serializable]
	public sealed class TpVector {
		public float x, y;
	
		public override string ToString() {
			return string.Format("[TpVector: x={0}, y={1}]", x, y);
		}
	}

	[Serializable]
	public sealed class TpRect {
		public float x, y, w, h;

		public override string ToString() {
			return string.Format("[TpRect: x={0}, y={1}, w={2}, h={3}]", x, y, w, h);
		}
	}

	[Serializable]
	public sealed class TpFrame {
		public string name;
		public TpVector size, pivot;
		public bool rotated, sliced;
		public TpRect quad, uv, border;

		[NonSerialized]
		public TpAtlas atlas;
		[NonSerialized]
		public Vector2 rectLeftBottom, rectLeftTop, rectRightTop, rectRightBottom;
		[NonSerialized]
		public Vector2 uvLeftBottom, uvLeftTop, uvRightTop, uvRightBottom;


		public void Init(TpAtlas atlas) {
			this.atlas = atlas;

			CalculateVertices(
				quad.x, quad.y, quad.w, quad.h,
				ref rectLeftBottom, ref rectLeftTop, ref rectRightTop, ref rectRightBottom);

			atlas.CalculateUvsInsideAtlas(
				rotated, uv.x, uv.y, uv.w, uv.h,
				ref uvLeftBottom, ref uvLeftTop, ref uvRightTop, ref uvRightBottom);
		}

		public void CalculateVertices(
			float quadX, float quadY, float quadW, float quadH,
			ref Vector2 rectLeftBottom, ref Vector2 rectLeftTop, ref Vector2 rectRightTop, ref Vector2 rectRightBottom) {

			float rectLeft = quadX;
			float rectRight = quadX + quadW;
			float rectTop = size.y - quadY;
			float rectBottom = size.y - (quadY + quadH);

			float pivotX = pivot.x;
			float pivotY = size.y - pivot.y;

			rectLeftBottom.Set(rectLeft - pivotX, rectBottom - pivotY);
			rectLeftTop.Set(rectLeft - pivotX, rectTop - pivotY);
			rectRightTop.Set(rectRight - pivotX, rectTop - pivotY);
			rectRightBottom.Set(rectRight - pivotX, rectBottom - pivotY);		
		}

		public void CalculateVertices(float z, ref Vector3 rectLeftBottom, ref Vector3 rectLeftTop, ref Vector3 rectRightTop, ref Vector3 rectRightBottom) {
			float rectLeft = quad.x;
			float rectRight = quad.x + quad.w;
			float rectTop = size.y - quad.y;
			float rectBottom = size.y - (quad.y + quad.h);

			float pivotX = pivot.x;
			float pivotY = size.y - pivot.y;

			rectLeftBottom.Set(rectLeft - pivotX, rectBottom - pivotY, z);
			rectLeftTop.Set(rectLeft - pivotX, rectTop - pivotY, z);
			rectRightTop.Set(rectRight - pivotX, rectTop - pivotY, z);
			rectRightBottom.Set(rectRight - pivotX, rectBottom - pivotY, z);		
		}

		public void CalculateUvsInsideFrame(
			float glyphX, float glyphY, float glyphW, float glyphH,
			ref Vector2 uvLeftBottom, ref Vector2 uvLeftTop, ref Vector2 uvRightTop, ref Vector2 uvRightBottom) {

			float frameX = uv.x + (!rotated ? glyphX : (uv.w - glyphY - glyphH));
			float frameY = uv.y + (!rotated ? glyphY : glyphX);

			atlas.CalculateUvsInsideAtlas(
				rotated, frameX, frameY, glyphW, glyphH, 
				ref uvLeftBottom, ref uvLeftTop, ref uvRightTop, ref uvRightBottom);
		}

		public override string ToString() {
			return string.Format("[TpFrame: name={0}, size={1}, pivot={2}, rotated={3}, sliced={4}, quad={5}, uv={6}, border={7}]", name, size, pivot, rotated, sliced, quad, uv, border);
		}
	}

	[Serializable]
	public sealed class TpAtlas {
		public string name;
		public TpVector size;
		public TpFrame[] frames;

		[NonSerialized]
		public Texture2D texture;

		public readonly Dictionary<string, TpFrame> frameByName = new Dictionary<string, TpFrame>();

		public void Init(Texture2D texture) {
			this.texture = texture;

			foreach (var frame in frames) {
				frame.Init(this);
				frameByName[frame.name] = frame;
			}
		}

		public void CalculateUvsInsideAtlas(
			bool rotated, float frameX, float frameY, float frameW, float frameH, 
			ref Vector2 uvLeftBottom, ref Vector2 uvLeftTop, ref Vector2 uvRightTop, ref Vector2 uvRightBottom) {

			float width = size.x;
			float height = size.y;

			float uvLeft = frameX / width;
			float uvRight = (frameX + frameW) / width;
			float uvTop = 1f - frameY / height;
			float uvBottom = 1f - (frameY + frameH) / height;

			if (!rotated) {
				uvLeftBottom.Set(uvLeft, uvBottom);
				uvLeftTop.Set(uvLeft, uvTop);	
				uvRightTop.Set(uvRight, uvTop);
				uvRightBottom.Set(uvRight, uvBottom);
			} else {  // Is rotated
				uvLeftBottom.Set(uvLeft, uvTop);	
				uvLeftTop.Set(uvRight, uvTop);
				uvRightTop.Set(uvRight, uvBottom);
				uvRightBottom.Set(uvLeft, uvBottom);
			}
		}

		public override string ToString() {
			return string.Format("[TpAtlas: name={0}, frames={1}]", name, frames);
		}
	}
}

