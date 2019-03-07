namespace Futilef.V2 {
	public class TpSprite : Drawable {
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

		public Vec4 color;
		public string spriteName;
		public float originalAspectRatio;
		TpDataFile.Sprite spriteData;

		public TpSprite(TpDataFile file, Shader shader, Texture texture) {
			this.file = file;
			this.shader = shader;
			this.texture = texture;

			color.One();
		}

		public void SetSprite(string spriteName) {
			this.spriteName = spriteName;
			spriteData = file.spriteDict[spriteName];
			originalAspectRatio = spriteData.size.x / spriteData.size.y;
		}

		protected override DrawNode CreateDrawNode() {
			return new Node{shader = shader, texture = texture};
		}

		protected override void UpdateDrawNode(DrawNode node) {
			if (hasTransformChanged) UpdateTransform();

			var n = (Node)node;
			n.color = color;

			n.uvQuad = spriteData.uvQuad;

			if (useLayout) {
				n.quad = cachedMatConcat * new Quad(0, 0, cachedRealSize.x, cachedRealSize.y);
			} else {
				n.quad = cachedMatConcat * new Quad(spriteData.rect);
			}
		}
	}
}
