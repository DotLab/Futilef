﻿namespace Futilef.V2 {
	public sealed class BmLabel : Drawable {
		public sealed class Node : DrawNode {
			public struct CharDrawInfo {
				public Quad quad;
				public Quad uvQuad;

				public Texture texture;
			}

			public Shader shader;
			public Vec4 color;

			public int charLen;
			public int charCount;
			public CharDrawInfo[] chars;

			public bool hasShadow;
			public Vec2 shadowPos;
			public Vec4 shadowColor;

			public override void Draw(DrawCtx ctx, int g) {
				if (g < 0) g = ctx.NewGroup();

				if (!hasShadow) {
					for (int i = 0; i < charCount; i += 1) {
						var b = ctx.GetBatch(shader, chars[i].texture, g);
						b.DrawQuad(chars[i].quad, chars[i].uvQuad, color);
					}
				} else {
					for (int i = 0; i < charCount; i += 1) {
						var b = ctx.GetBatch(shader, chars[i].texture, g);
						b.DrawQuad(chars[i].quad + shadowPos, chars[i].uvQuad, shadowColor);
						b.DrawQuad(chars[i].quad, chars[i].uvQuad, color);
					}
				}
			}
		}

		public static class AlignMode {
			public const int alignBase = 0;
			public const int alignLine = 1;
			public const int alignMesh = 2;
		}

		public readonly BmFontFile file;
		public readonly Shader shader;
		public readonly IStore<Texture> textureStore;

		public bool hasShadow;

		public Vec4 color;

		public Vec2 shadowPos;
		public Vec4 shadowColor;

		public int textLen;
		public string text;
		public int textAlign;
		public int alignMode;

		public float fontSize;

		public BmLabel(BmFontFile file, Shader shader, IStore<Texture> textureStore) {
			this.file = file;
			this.shader = shader;
			this.textureStore = textureStore;

			for (int i = 0; i < file.pages; i += 1) {
				textureStore.Get(file.pageNames[i]);
			}

			color.One();
		}

		public void SetText(string text) {
			if (this.text == text) return;
			this.text = text;
			textLen = text.Length;
		}

		public void SetFontSize(float fontSize) {
			if (this.fontSize == fontSize) return;
			this.fontSize = fontSize;
		}

		protected override DrawNode CreateDrawNode() { 
			return new Node{shader = shader}; 
		}

		protected override void UpdateDrawNode(DrawNode node) {
			if (hasTransformChanged) UpdateTransform();

			var n = (Node)node;
			n.color = color;

			n.hasShadow = hasShadow;
			if (hasShadow) {
				n.shadowPos = shadowPos;
				n.shadowColor = shadowColor;
			}

			if (n.chars == null || n.charLen < textLen) {
				n.chars = new Node.CharDrawInfo[textLen];
				n.charLen = textLen;
			}

			float fontScaling = fontSize / (float)file.fontSize;
			var lineDrawInfo = file.GenerateDrawInfo(text);

			Vec2 textPivotPos;
			var textPivot = Align.Calc(textAlign);
			if (textAlign != Align.none) {
				if (alignMode == AlignMode.alignBase) {
					textPivotPos = new Vec2(
						(lineDrawInfo.size.w + lineDrawInfo.size.x) * textPivot.x, 
						(lineDrawInfo.size.h + lineDrawInfo.size.y) * textPivot.y);
				} else if (alignMode == AlignMode.alignLine) {
					textPivotPos = new Vec2(
						lineDrawInfo.size.w * textPivot.x + lineDrawInfo.size.x, 
						lineDrawInfo.size.h * textPivot.y + lineDrawInfo.size.y);
				} else {
					textPivotPos = new Vec2(
						lineDrawInfo.meshSize.w * textPivot.x + lineDrawInfo.meshSize.x, 
						lineDrawInfo.meshSize.h * textPivot.y + lineDrawInfo.meshSize.y);
				}
				textPivotPos.Mult(fontScaling);
			} else {
				textPivotPos = new Vec2();
			}

			if (useLayout) {
				textPivotPos.Sub(textPivot * cachedRealSize);
			}

			for (int i = 0, end = lineDrawInfo.charDrawInfos.Length; i < end; i++) {
				var info = lineDrawInfo.charDrawInfos[i];
				n.chars[i].quad = cachedMatConcat * (new Quad(info.rect * fontScaling) - textPivotPos);
				n.chars[i].uvQuad.FromRect(info.uvRect);
				n.chars[i].texture = textureStore.Get(file.pageNames[info.page]);
			}
			n.charCount = lineDrawInfo.charDrawInfos.Length;
		}
	}
}

