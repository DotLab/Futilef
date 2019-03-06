namespace Futilef.V2 {
	public abstract class Drawable {
		public Vec2 pos;
		public float rotZ;
		public bool matDirty;

		public Mat2D mat;
		public Mat2D matConcat;
		public Mat2D matConcatInverse;
		public bool needMatConcatInverse;

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
		protected virtual void UpdateDrawNode(DrawNode node) {
			if (matDirty) {

			}
		}
	}
}

