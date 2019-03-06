namespace Futilef.V2 {
	public abstract class Drawable {
		public readonly DrawNode[] drawNodes = new DrawNode[3];

		public virtual DrawNode GenerateDrawNodeSubtree(int index) {
			var node = drawNodes[index];
			if (node == null) {
				node = drawNodes[index] = CreateDrawNode();
			}
			UpdateDrawNode(node);
			return node;
		}

		protected abstract DrawNode CreateDrawNode();
		protected virtual void UpdateDrawNode(DrawNode node) {}
	}
}

