namespace Futilef.V2 {
	public abstract class Drawable {
		public uint age;
		public readonly DrawNode[] drawNodes = new DrawNode[3];

		// this is called in update thread
		public virtual bool Update() {
			return true;
		}

		public virtual DrawNode GetDrawNode(int index) {
			var node = drawNodes[index] ?? (drawNodes[index] = CreateDrawNode());
			if (node.age != age) UpdateDrawNode(node);
			return node;
		}

		protected abstract DrawNode CreateDrawNode();

		protected virtual void UpdateDrawNode(DrawNode node) {
			node.age = age;
		}
	}
}

