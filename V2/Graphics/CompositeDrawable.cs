using System.Collections.Generic;

namespace Futilef.V2 {
	public class CompositeDrawable : Drawable {
		public class Node : DrawNode {
			public readonly List<DrawNode> children = new List<DrawNode>();

			public override void Draw(DrawCtx ctx, int g) {
				
			}
		}

		public readonly List<Drawable> children = new List<Drawable>();

		public override DrawNode GenerateDrawNodeSubtree(int index) {
			var n = (Node)base.GenerateDrawNodeSubtree(index);
			n.children.Clear();

			for (int i = 0, end = children.Count; i < end; i++) {
				n.children.Add(children[i].GenerateDrawNodeSubtree(index));
			}

			return n;
		}

		protected override DrawNode CreateDrawNode() {
			return new Node();
		}

		protected override void UpdateDrawNode(DrawNode node) {}
	}
}

