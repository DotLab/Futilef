namespace Futilef.V2 {
	public abstract class Drawable {
		public uint age;

		public virtual bool Update() {
			return true;
		}

		public readonly DrawNode[] drawNodes = new DrawNode[3];

		public virtual DrawNode GenerateDrawNode(int index, double deltaTime) {
			var node = drawNodes[index] ?? (drawNodes[index] = CreateDrawNode());
			if (node.age != age) ApplyDrawNode(node);
			return node;
		}

		protected virtual DrawNode CreateDrawNode() {
			return new DrawNode();
		}

		protected virtual void ApplyDrawNode(DrawNode node) {
			node.age = age;
		}
	}
}

