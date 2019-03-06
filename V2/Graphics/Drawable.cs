namespace Futilef.V2 {
	public abstract class Drawable {
		public Vec2 pos;
		public Vec2 scl;
		public float rotZ;
		public bool isMatDirty;

		public CompositeDrawable parent;

		public Mat2D mat;
		public Mat2D matConcat;
		public Mat2D matConcatInverse;
		public bool needMatConcatInverse;

		public readonly DrawNode[] drawNodes = new DrawNode[3];

		protected Drawable() {
			scl = new Vec2(1);
			isMatDirty = true;
		}

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

		protected virtual void UpdateMatIfDirty() {
			if (isMatDirty) {
				isMatDirty = false;

				mat.FromScalingRotationTranslation(pos, rotZ, scl);
				matConcat = parent == null ? mat : parent.matConcat * mat;
				if (needMatConcatInverse) matConcatInverse.FromInverting(matConcat);
			}
		}
	}
}

