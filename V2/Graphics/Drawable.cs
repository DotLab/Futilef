namespace Futilef.V2 {
	public abstract class Drawable {
		public bool useLayout;

		public int anchorAlign;
		public Vec2 customAnchorAlign;
		public int pivotAlign;
		public Vec2 customPivotAlign;

		public Vec2 pos;
		public int relativePosAxes;

		public Vec2 size;
		public int relativeSizeAxes;

		public Drawable parent;

		public Vec2 scl;
		public float rotZ;
		public bool hasTransformChanged;

		public Vec2 cachedAnchor;
		public Vec2 cachedPivot;
		public Vec2 cachedReadPos;
		public Vec2 cachedRealSize;

		public Mat2D cachedMat;
		public Mat2D cachedMatConcat;

		public readonly DrawNode[] drawNodes = new DrawNode[3];

		protected Drawable() {
			scl = new Vec2(1);
			hasTransformChanged = true;
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

		public virtual void UpdateTransform() {
			hasTransformChanged = false;

			bool useParentSize = parent != null && parent.useLayout;
			if (useParentSize) {
				cachedReadPos = CalcAbsoluteVal(relativePosAxes, pos, parent.cachedRealSize);
				cachedRealSize = CalcAbsoluteVal(relativeSizeAxes, size, parent.cachedRealSize);
			} else {
				cachedReadPos = CalcAbsoluteVal(relativePosAxes, pos);
				cachedRealSize = CalcAbsoluteVal(relativeSizeAxes, size);
			}

			if (useLayout) {
				cachedPivot = cachedRealSize * Align.Calc(pivotAlign, customPivotAlign);
				cachedMat.FromTranslation(-cachedPivot);
				cachedMat.ScaleRotateTranslate(scl, rotZ, cachedReadPos);
			} else {
				cachedMat.FromScalingRotationTranslation(scl, rotZ, cachedReadPos);
			}

			if (useParentSize) {
				cachedAnchor = parent.cachedRealSize * Align.Calc(anchorAlign, customAnchorAlign);
				cachedMat.Translate(cachedAnchor);
			}
				
			cachedMatConcat = parent == null ? cachedMat : parent.cachedMatConcat * cachedMat;
//			if (needMatConcatInverse) matConcatInverse.FromInverting(matConcat);
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

	public static class Align {
		public const int none = 0;

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

		public static Vec2 Calc(int alignment, Vec2 value) {
			if (alignment == Align.custom) return value;

			if ((alignment & Align.left) != 0) value.x = 0;
			else if ((alignment & Align.centerH) != 0) value.x = .5f;
			else if ((alignment & Align.right) != 0) value.x = 1;

			if ((alignment & Align.top) != 0) value.y = 1;
			else if ((alignment & Align.centerV) != 0) value.y = .5f;
			else if ((alignment & Align.bottom) != 0) value.y = 0;
			return value;
		}

		public static Vec2 Calc(int alignment) {
			return Calc(alignment, new Vec2());
		}
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

