namespace Futilef.V2 {
	public class BmLabel : Drawable {
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

		public static class VerticalAlign {
			public const int Base = 0;
			public const int Line = 1;
			public const int Mesh = 2;
		}

		public readonly BmFontFile file;
		public readonly Shader shader;
		public readonly IStore<Texture> textureStore;

		public bool hasShadow;

		public Vec2 shadowPos;
		public Vec4 shadowColor;

		public string text;

		public float fontSize;
		public int textAlign = Align.BottomLeft;
		public int verticalAlign = VerticalAlign.Base;

		public bool textDirty;
		public bool charTransformDirty;

		BmFontFile.LineDrawInfo lineDrawInfo;
		float fontScaling;
		Vec2 textPivotPos;

		public BmLabel(BmFontFile file, Shader shader, IStore<Texture> textureStore) {
			this.file = file;
			this.shader = shader;
			this.textureStore = textureStore;

			for (int i = 0; i < file.pages; i += 1) {
				textureStore.Get(file.pageNames[i]);
			}

			fontSize = file.fontSize;
		}

		protected override DrawNode CreateDrawNode() { 
			return new Node{shader = shader}; 
		}

		protected override void UpdateDrawNode(DrawNode node) {
			base.UpdateDrawNode(node);

			var n = (Node)node;
			if (string.IsNullOrEmpty(text)) {
				n.charCount = 0;
				return;
			}

			if (textDirty) {
				lineDrawInfo = file.GenerateDrawInfo(text);
			}

			if (textDirty || charTransformDirty) {
				textDirty = charTransformDirty = false;

				fontScaling = fontSize / (float)file.fontSize;
				var textPivot = Align.Calc(textAlign);
				if (textAlign != Align.None) {
					if (verticalAlign == VerticalAlign.Base) {
						textPivotPos = new Vec2(
							(lineDrawInfo.size.w + lineDrawInfo.size.x) * textPivot.x, 
							(lineDrawInfo.size.h + lineDrawInfo.size.y) * textPivot.y);
					} else if (verticalAlign == VerticalAlign.Line) {
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
					textPivotPos.Sub(textPivot * cachedSize);
				}
			}

			n.color = cachedColor;

			n.hasShadow = hasShadow;
			if (hasShadow) {
				n.shadowPos = cachedMatConcat * shadowPos - cachedMatConcat * new Vec2();
				n.shadowColor = shadowColor;
				n.shadowColor.w *= alpha;
			}

			var textLen = text.Length;
			if (n.chars == null || n.charLen < textLen) {
				n.chars = new Node.CharDrawInfo[textLen];
				n.charLen = textLen;
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
			self.charTransformDirty = true; self.age += 1; return self;
		}

		public static T TextShadow<T>(this T self, float x, float y, Vec4 color) where T : BmLabel {
			self.shadowPos.Set(x, y); self.shadowColor = color; self.hasShadow = true;
			self.age += 1; return self;
		}
	}
}

