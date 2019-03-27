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

		// non-cachable
//		public bool flattenSubtree = true;
		public bool flattenSubtree;

		public CompositeDrawable() {
			this.handleInput = true;
		}

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

		public override void CacheTransform() {
			base.CacheTransform();

			for (int i = 0, end = children.Count; i < end; i++) {
				var child = children[i];
				child.parent = this;
				child.CacheTransform();
				child.age += 1;
			}
		}

		public override void CacheColor() {
			base.CacheColor();

			for (int i = 0, end = children.Count; i < end; i++) {
				var child = children[i];
				child.CacheColor();
				child.age += 1;
			}
		}

		public Drawable PropagateChildren(UiEvent e) {
			for (int i = children.Count - 1; i >= 0; i--) {
				var child = children[i];
				if (child.handleInput) {
					var focus = child.Propagate(e);
					if (focus != null) return focus;
				}
			}
			return null;
		}

		public override Drawable Propagate(UiEvent e) {
			// reverse propagation
//			Console.Log(GetType(), e.GetType());
			return PropagateChildren(e) ?? base.Propagate(e);
		}
	}

	public static class CompositeDrawableExtension {
		public static T AddChild<T>(this T self, Drawable child) where T : CompositeDrawable {
			self.Add(child); return self;
		}
	}
}

