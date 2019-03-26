namespace Futilef.V2 {
	public sealed class TpSpriteSliced : Drawable {
		public sealed class Node : DrawNode {
			public Shader shader;
			public Texture texture;
			public Vec4 color;

			public Quad quad;
			public Quad quadInner;
			public Quad uvQuad;
			public Quad uvQuadInner;

			public override void Draw(DrawCtx ctx, int g) {
				var b = ctx.GetBatch(shader, texture);
				b.DrawQuad9Sliced(quad, quadInner, uvQuad, uvQuadInner, color);
			}
		}

		public readonly TpDataFile file;
		public readonly Shader shader;
		public readonly Texture texture;

		// sprite
		public bool spriteDirty;
		public string spriteName;
		public float spriteScale;

		// cache
		public TpDataFile.Sprite cachedSpriteData;
		public Border cachedBorder;

		public TpSpriteSliced(TpDataFile file, Shader shader, Texture texture) {
			this.file = file;
			this.shader = shader;
			this.texture = texture;
		}

		protected override DrawNode CreateDrawNode() { return new Node{shader = shader, texture = texture}; }

		protected override void UpdateDrawNode(DrawNode node) {
			base.UpdateDrawNode(node);

			if (spriteDirty) {
				spriteDirty = false;

				cachedSpriteData = file.spriteDict[spriteName];
				cachedBorder = cachedSpriteData.border * spriteScale;
			}

			var n = (Node)node;
			n.color = cachedColor;
			n.uvQuad = cachedSpriteData.uvQuad;
			n.uvQuadInner = cachedSpriteData.uvQuadInner;
			if (useLayout) {
				n.quad = cachedMatConcat * new Quad(0, 0, cachedSize.x, cachedSize.y);
				n.quadInner = cachedMatConcat * new Quad(
					cachedBorder.l, 
					cachedBorder.b, 
					cachedSize.x - cachedBorder.l - cachedBorder.r, 
					cachedSize.y - cachedBorder.t - cachedBorder.b);
			} else {
				n.quad = cachedMatConcat * new Quad(cachedSpriteData.rect);
				n.quadInner = cachedMatConcat * new Quad(cachedSpriteData.rectInner);

			}
			Console.Log("uv", cachedSpriteData.uvQuad, "inner", cachedSpriteData.uvQuadInner);
		}
	}

	public static class TpSpriteSlicedExtension {
		public static TpSpriteSliced Sprite(this TpSpriteSliced self, string spriteName, float spriteScale = 1) {
			self.spriteName = spriteName; self.spriteScale = spriteScale;
			self.spriteDirty = true; self.age += 1; return self;
		}

		public static TpSpriteSliced SpriteScale(this TpSpriteSliced self, float spriteScale) {
			self.spriteScale = spriteScale;
			self.spriteDirty = true; self.age += 1; return self;
		}
	}
}

