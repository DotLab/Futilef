namespace Futilef.V2 {
	public class BmLabel : Drawable {
		public sealed class Node : DrawNode {
			public Shader shader;
			public Vec4 color;

			public int charInfoCount;
			public CharInfo[] charInfos;

			public bool hasShadow;
			public Vec2 shadowPos;
			public Vec4 shadowColor;

			public override void Draw(DrawCtx ctx, int g) {
				if (g < 0) g = ctx.NewGroup();

				if (!hasShadow) {
					for (int i = 0; i < charInfoCount; i += 1) {
						var b = ctx.GetBatch(shader, charInfos[i].texture, g);
						b.DrawQuad(charInfos[i].quad, charInfos[i].uvQuad, color);
					}
				} else {
					for (int i = 0; i < charInfoCount; i += 1) {
						var b = ctx.GetBatch(shader, charInfos[i].texture, g);
						b.DrawQuad(charInfos[i].quad + shadowPos, charInfos[i].uvQuad, shadowColor);
						b.DrawQuad(charInfos[i].quad, charInfos[i].uvQuad, color);
					}
				}
			}
		}

		public struct CharInfo {
			public Quad quad;
			public Quad uvQuad;
			public Texture texture;
		}

		public static class VerticalAlign {
			public const int Base = 0;
			public const int Line = 1;
			public const int Mesh = 2;
		}

		public readonly BmFontFile file;
		public readonly Texture[] textures;

		public readonly Shader shader;

		// text
		public bool textDirty;
		public string text;

		// text transform
		public bool textTransformDirty;
		public float fontSize;
		public int textAlign = Align.BottomLeft;
		public int verticalAlign = VerticalAlign.Base;

		// non-cachable
		public bool hasShadow;
		public Vec2 shadowPos;
		public Vec4 shadowColor;

		// cache
		public Rect cachedLineRect;
		public Rect cachedLineMeshRect;

		public int cachedCharInfoCount;
		public CharInfo[] cachedCharInfos;

		public Vec2 cachedTextPivotPos;

		public BmLabel(BmFontFile file, Shader shader, IStore<Texture> textureStore) {
			this.file = file;
			this.shader = shader;

			fontSize = file.fontSize;

			textures = new Texture[file.pages];
			for (int i = 0; i < file.pages; i += 1) textures[i] = textureStore.Get(file.pageNames[i]);
		}

		protected override DrawNode CreateDrawNode() { return new Node{shader = shader}; }

		protected override void UpdateDrawNode(DrawNode node) {
			base.UpdateDrawNode(node);

			if (textDirty) CacheText();
			if (textTransformDirty) CacheTextTransform();

			var n = (Node)node;
			if (string.IsNullOrEmpty(text)) {
				n.charInfoCount = 0;
				return;
			}

			n.color = cachedColor;

			n.hasShadow = hasShadow;
			if (hasShadow) {
				n.shadowPos = cachedMatConcat * shadowPos - cachedMatConcat * new Vec2();
				n.shadowColor = shadowColor;
				n.shadowColor.w *= alpha;
			}

			if (n.charInfos == null || n.charInfos.Length < cachedCharInfoCount) {
				n.charInfos = new CharInfo[cachedCharInfoCount];
			}
			n.charInfoCount = cachedCharInfoCount;

			var fontScaling = fontSize / file.fontSize;
			for (int i = 0; i < cachedCharInfoCount; i++) {
				var info = cachedCharInfos[i];
				n.charInfos[i].quad = cachedMatConcat * (info.quad * fontScaling - cachedTextPivotPos);
				n.charInfos[i].uvQuad = info.uvQuad;
				n.charInfos[i].texture = info.texture;
			}
		}

		public void CacheText() {
			textDirty = false;

			int textLength = text.Length;
			if (cachedCharInfos == null || cachedCharInfos.Length < textLength) {
				cachedCharInfos = new CharInfo[textLength];
			}

			int j = 0;
			int curX = 0;
			uint lastChar = 0;

			float meshLeft = 0;
			float meshRight = 0;
			float meshTop = 0;
			float meshBottom = 0;

			for (int i = 0; i < textLength; i++) {
				BmFontFile.Glyph g;
				char c = text[i];
				if (!file.glyphDict.TryGetValue(c, out g)) continue;

				if (char.IsWhiteSpace(c)) {
					curX += g.xAdvance;
					continue;
				}

				if (j > 0) curX += file.GetKerning(lastChar, c);

				float left = curX + g.xOffset;
				float right = left + g.width;
				float top = file.lineBase - g.yOffset;
				float bottom = top - g.height;

				if (j == 0) {
					meshLeft = left;
					meshRight = right;
					meshTop = top;
					meshBottom = bottom;
				} else {
					if (left < meshLeft) meshLeft = left;
					if (right > meshRight) meshRight = right;
					if (top > meshTop) meshTop = top;
					if (bottom < meshBottom) meshBottom = bottom;
				}

				cachedCharInfos[j].quad.Set(left, bottom, g.width, g.height);
				cachedCharInfos[j].uvQuad.Set(g.uvRect);
				cachedCharInfos[j].texture = textures[g.page];

				curX += g.xAdvance;
				lastChar = c;
				j += 1;
			}

			cachedCharInfoCount = j;
			cachedLineRect = new Rect(0, file.lineBase - file.lineHeight, curX, file.lineBase);
			cachedLineMeshRect = new Rect(meshLeft, meshBottom, meshRight - meshLeft, meshTop - meshBottom);

			// need recalculate line size
			CacheTextTransform();
		}

		public void CacheTextTransform() {
			textTransformDirty = false;

			var textPivot = Align.Calc(textAlign);
			if (textAlign != Align.None) {
				if (verticalAlign == VerticalAlign.Base) {
					cachedTextPivotPos.Set(
						(cachedLineRect.w + cachedLineRect.x) * textPivot.x, 
						(cachedLineRect.h + cachedLineRect.y) * textPivot.y);
				} else if (verticalAlign == VerticalAlign.Line) {
					cachedTextPivotPos.Set(
						cachedLineRect.w * textPivot.x + cachedLineRect.x, 
						cachedLineRect.h * textPivot.y + cachedLineRect.y);
				} else {
					cachedTextPivotPos.Set(
						cachedLineMeshRect.w * textPivot.x + cachedLineMeshRect.x, 
						cachedLineMeshRect.h * textPivot.y + cachedLineMeshRect.y);
				}
				cachedTextPivotPos.Mult(fontSize / file.fontSize);
			} else {
				cachedTextPivotPos.Set(0);
			}

			if (useLayout) {
				cachedTextPivotPos.Sub(textPivot * cachedSize);
			}
		}
	}

	public static class BmLabelExtension {
		public static T Text<T>(this T self, string text) where T : BmLabel {
			self.text = text;
			self.textDirty = true; self.age += 1; return self;
		}

		public static T Text<T>(this T self, float fontSize, string text) where T : BmLabel {
			self.fontSize = fontSize; self.text = text;
			self.textDirty = true; self.age += 1; return self;
		}

		public static T TextAlign<T>(this T self, int textAlign, int verticalAlign) where T : BmLabel {
			self.textAlign = textAlign; self.verticalAlign = verticalAlign;
			self.textTransformDirty = true; self.age += 1; return self;
		}

		public static T TextShadow<T>(this T self, float x, float y, Vec4 color) where T : BmLabel {
			self.shadowPos.Set(x, y); self.shadowColor = color; self.hasShadow = true;
			self.age += 1; return self;
		}
	}
}

