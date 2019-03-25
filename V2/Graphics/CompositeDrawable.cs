using System.Collections.Generic;

namespace Futilef.V2 {
	public class CompositeDrawable : Drawable {
		public class Node : DrawNode {
			public readonly List<DrawNode> children = new List<DrawNode>();
			public Quad quad;

			public override void Draw(DrawCtx ctx, int g) {
				var b = ctx.GetBatch(ctx.debugShader, ctx.debugTexture);
				b.DrawQuad(quad, new Quad(), new Vec4(1, 0, 1, .2f));

				for (int i = 0, end = children.Count; i < end; i++) {
					children[i].Draw(ctx, g);
				}
			}
		}

		public readonly List<Drawable> children = new List<Drawable>();
//		public bool flattenSubtree = true;
		public bool flattenSubtree;

		public override DrawNode GenerateDrawNodeSubtree(int index) {
			var n = (Node)base.GenerateDrawNodeSubtree(index);

			n.children.Clear();
			AddDrawNodeSubtreeRecur(index, this, n.children);

			return n;
		}

		static void AddDrawNodeSubtreeRecur(int index, CompositeDrawable parentDrawable, List<DrawNode> drawNodes) {
			for (int i = 0, end = parentDrawable.children.Count; i < end; i++) {
				var child = parentDrawable.children[i];
				var compositeChild = child as CompositeDrawable;
				if (compositeChild != null && parentDrawable.flattenSubtree) {
					AddDrawNodeSubtreeRecur(index, compositeChild, drawNodes);
				} else {
					drawNodes.Add(child.GenerateDrawNodeSubtree(index));
				}
			}
		}

		public virtual void Add(Drawable child) {
			children.Add(child);
			age += 1;
		}

		protected override DrawNode CreateDrawNode() { return new Node(); }

		protected override void UpdateDrawNode(DrawNode node) {
			base.UpdateDrawNode(node);

			var n = (Node)node;
			if (useLayout) {
				n.quad = cachedMatConcat * new Quad(0, 0, cachedSize.x, cachedSize.y);
			}
		}

		public override void UpdateTransform() {
			base.UpdateTransform();
			for (int i = 0, end = children.Count; i < end; i++) {
				var child = children[i];
				child.parent = this;
				child.UpdateTransform();
				child.age += 1;
			}
			Console.Log(GetType(), cachedPos, cachedSize, scl);
		}

		public override void UpdateColor() {
			base.UpdateColor();
			for (int i = 0, end = children.Count; i < end; i++) {
				var child = children[i];
				child.UpdateColor();
				child.age += 1;
			}
		}

		public override bool Propagate(UiEvent e) {
			// reverse propagation
			for (int i = children.Count - 1; i > 0; i--) {
				var child = children[i];
				if (child.handleInput && child.Propagate(e)) return true;
			}
			return base.Propagate(e);
		}
	}
}

