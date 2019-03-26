namespace Futilef.V2 {
	public sealed class TpSprite : Drawable {
		public sealed class Node : DrawNode {
			public Shader shader;
			public Texture texture;
			public Vec4 color;

			public Quad quad;
			public Quad uvQuad;

			public override void Draw(DrawCtx ctx, int g) {
				var b = ctx.GetBatch(shader, texture);
				b.DrawQuad(quad, uvQuad, color);
			}
		}

		public readonly TpDataFile file;
		public readonly Shader shader;
		public readonly Texture texture;

		public string spriteName;
		public bool spriteNameDirty;

		float originalAspectRatio;
		TpDataFile.Sprite spriteData;

		public TpSprite(TpDataFile file, Shader shader, Texture texture) {
			this.file = file;
			this.shader = shader;
			this.texture = texture;
		}

		protected override DrawNode CreateDrawNode() { return new Node{shader = shader, texture = texture}; }

		protected override void UpdateDrawNode(DrawNode node) {
			if (transformDirty) CacheTransform();
			if (colorDirty) CacheColor();

			if (spriteNameDirty) {
				spriteNameDirty = false;

				spriteData = file.spriteDict[spriteName];
				originalAspectRatio = spriteData.size.x / spriteData.size.y;
			}

			var n = (Node)node;
			n.color = cachedColor;
			n.uvQuad = spriteData.uvQuad;
			if (useLayout) {
				n.quad = cachedMatConcat * new Quad(0, 0, cachedSize.x, cachedSize.y);
			} else {
				n.quad = cachedMatConcat * new Quad(spriteData.rect);
			}
		}
	}

	public static class TpSpriteExtension {
		public static TpSprite Sprite(this TpSprite self, string spriteName) {
			self.spriteName = spriteName; 
			self.spriteNameDirty = true; self.age += 1; return self;
		}
	}
}
