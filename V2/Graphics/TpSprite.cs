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

		public TpSprite(TpDataFile file, Shader shader, Texture texture) {
			this.file = file;
			this.shader = shader;
			this.texture = texture;

			color.One();
		}

		public void ChooseSprite(string spriteName) {
			this.spriteName = spriteName;
		}

		protected override DrawNode CreateDrawNode() {
			return new Node{shader = shader, texture = texture};
		}

		protected override void UpdateDrawNode(DrawNode node) {
			var n = (Node)node;
			n.color = color;

			var sprite = file.spriteDict[spriteName];
			n.quad.FromRect(sprite.rect);
			n.uvQuad = sprite.uvQuad;
		}
	}
}
