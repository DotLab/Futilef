using System;

namespace Futilef.V2 {
	public sealed class BmLabel : Drawable {
		public sealed class Node : DrawNode {
			public struct CharDrawInfo {
				public Quad uvQuad;
				public Quad quad;

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

		public readonly BmFontFile file;
		public readonly Shader shader;
		public readonly IStore<Texture> textureStore;

		public bool hasShadow;

		public Vec4 color;

		public Vec2 shadowPos;
		public Vec4 shadowColor;

		public int textLen;
		public string text;

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
			if (isMatDirty) UpdateMat();

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

			int j = 0;
			int curX = 0;
			const int curY = 0;
			float fontScaling = fontSize / (float)file.fontSize;
			uint lastChar = 0;
			for (int i = 0; i < textLen; i += 1) {
				BmFontFile.Glyph g;

				char c = text[i];
				if (!file.glyphDict.TryGetValue(c, out g)) continue;

				if (Char.IsWhiteSpace(c)) {
					curX += g.xAdvance;
					continue;
				}

				n.chars[j].uvQuad.FromRect(g.uvRect);
				n.chars[j].texture = textureStore.Get(file.pageNames[g.page]);

				if (j > 0) curX += file.GetKerning(lastChar, c);

				int bottom = curY - g.yOffset - g.height;
				int left = curX + g.xOffset;
				/**
				 *        curY
				 *        |
				 *        posY
				 * curX - posX
				 */
				n.chars[j].quad = matConcat * new Quad(
					left * fontScaling,
					bottom * fontScaling,
					g.width * fontScaling,
					g.height * fontScaling);

				curX += g.xAdvance;

				lastChar = c;
				j += 1;
			}

			n.charCount = j;
		}
	}
}

