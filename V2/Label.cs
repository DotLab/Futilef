using System;
using Futilef.V2.Store;

namespace Futilef.V2 {
	public sealed class Label : Drawable {
		public sealed class LabelDrawNode : DrawNode {
			public struct CharDrawInfo {
				public Quad uvQuad;
				public Quad quad;

				public Texture texture;
			}

			public int charLen;
			public int charCount;
			public CharDrawInfo[] chars;

			public readonly Label owner;

			public LabelDrawNode(Label owner) {
				this.owner = owner;
			}

			public override void Draw(DrawCtx ctx) {
				int g = ctx.NewGroup();
				for (int i = 0; i < charCount; i += 1) {
					var b = ctx.GetBatch(owner.shader, chars[i].texture, g);
					b.DrawQuad(chars[i].quad, chars[i].uvQuad);
				}
			}
		}

		public readonly BmFontFile file;
		public readonly Shader shader;
		public readonly IStore<Texture> textureStore;

		public int textLen;
		public string text;

		public float fontSize;

		public Label(BmFontFile file, Shader shader, IStore<Texture> textureStore) {
			this.file = file;
			this.shader = shader;
			this.textureStore = textureStore;
		}

		public void SetText(string text) {
			if (this.text == text) return;
			this.text = text;
			textLen = text.Length;
			age += 1;
		}

		public void SetFontSize(float fontSize) {
			if (this.fontSize == fontSize) return;
			this.fontSize = fontSize;
			age += 1;
		}

		protected override DrawNode CreateDrawNode() {
			return new LabelDrawNode(this);
		}

		protected override void ApplyDrawNode(DrawNode node) {
			var n = (LabelDrawNode)node;
			n.age = age;

			if (n.chars == null || n.charLen < textLen) {
				n.chars = new LabelDrawNode.CharDrawInfo[textLen];
				n.charLen = textLen;
			}

			int j = 0;
			int curX = 0;
			int curY = 0;
			float fontScaling = fontSize / (float)file.fontSize;
			uint lastChar = 0;
			for (int i = 0; i < textLen; i += 1) {
				BmFontFile.Glyph g;
				uint c = text[i];
				if (!file.glyphDict.TryGetValue(c, out g)) continue;

				n.chars[j].uvQuad.FromRect(g.uvRect);
				n.chars[j].texture = textureStore.Get(file.pageNames[g.page]);

				if (j > 0) curX += file.GetKerning(lastChar, c);

				int top = curY - g.yOffset;
				int left = curX + g.xOffset;
				/**
				 * curY
				 * |
				 * posY
				 * curX - posX
				 */
				n.chars[j].quad.Set(
					(top)* fontScaling,
					(left + g.width) * fontScaling,
					(top - g.height) * fontScaling,
					(left) * fontScaling);

				curX += g.xAdvance;

				lastChar = c;
				j += 1;
			}

			n.charCount = j;
		}
	}
}

