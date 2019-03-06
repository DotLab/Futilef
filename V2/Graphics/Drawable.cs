namespace Futilef.V2 {
	public abstract class Drawable {
		public int anchor;
		public Vec2 customAnchor;
		public int pivot;
		public Vec2 customPivot;
		public bool useLayout;

		public Vec2 pos;
		public Vec2 absPos;
		public int relPosAxes;

		public Vec2 size;
		public Vec2 absSize;
		public int relSizeAxes;

		public Drawable parent;

		public Vec2 scl;
		public float rotZ;
		public bool isMatDirty;

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

		protected virtual void UpdateMat() {
			isMatDirty = false;

			bool useParentSize = parent != null && parent.useLayout;
			if (useParentSize) {
				absPos = CalcAbsoluteVal(relPosAxes, pos, parent.absSize);
				absSize = CalcAbsoluteVal(relSizeAxes, size, parent.absSize);
			} else {
				absPos = CalcAbsoluteVal(relPosAxes, pos);
				absSize = CalcAbsoluteVal(relSizeAxes, size);
			}

			if (useLayout) {
				var pivotPos = absSize * CalcAnchor(pivot, customPivot);
				mat.FromTranslation(-pivotPos);
				mat.ScaleRotateTranslate(scl, rotZ, absPos);
			} else {
				mat.FromScalingRotationTranslation(scl, rotZ, absPos);
			}

			if (useParentSize) {
				mat.Translate(parent.absSize * CalcAnchor(anchor, customAnchor));
			}
				
			matConcat = parent == null ? mat : parent.matConcat * mat;
//			if (needMatConcatInverse) matConcatInverse.FromInverting(matConcat);
		}

		public static Vec2 CalcAnchor(int anchor, Vec2 custom) {
			if (anchor == Anchor.custom) return custom;

			if ((anchor & Anchor.left) != 0) custom.x = 0;
			else if ((anchor & Anchor.centerH) != 0) custom.x = .5f;
			else if ((anchor & Anchor.right) != 0) custom.x = 1;

			if ((anchor & Anchor.top) != 0) custom.y = 1;
			else if ((anchor & Anchor.centerV) != 0) custom.y = .5f;
			else if ((anchor & Anchor.bottom) != 0) custom.y = 0;
			return custom;
		}

		public static Vec2 CalcAbsoluteVal(int axes, Vec2 val, Vec2 parentSize) {
			if ((axes & Axes.x) != 0) val.x *= parentSize.x;
			if ((axes & Axes.y) != 0) val.y *= parentSize.y;
			return val;
		}

		public static Vec2 CalcAbsoluteVal(int axes, Vec2 val) {
			if ((axes & Axes.x) != 0) val.x = 0;
			if ((axes & Axes.y) != 0) val.y = 0;
			return val;
		}
	}

	public static class Anchor {
		public const int topLeft = top | left;
		public const int topCenter = top | centerH;
		public const int topRight = top | right;

		public const int centerLeft = centerV | left;
		public const int center = centerV | centerH;
		public const int centerRight = centerV | right;

		public const int bottomLeft = bottom | left;
		public const int bottomCentre = bottom | centerH;
		public const int bottomRight = bottom | right;

		public const int top = 1 << 0;
		public const int centerV = 1 << 1;
		public const int bottom = 1 << 2;

		public const int left = 1 << 3;
		public const int centerH = 1 << 4;
		public const int right = 1 << 5;

		public const int custom = 1 << 6;
	}

	public static class Axes {
		public const int none = 0;
		public const int x = 1 << 0;
		public const int y = 1 << 1;
		public const int both = x | y;
	}

	public static class FillMode {
		public const int stretch = 0;
		public const int fill = 1;
		public const int fit = 2;
	}
}

