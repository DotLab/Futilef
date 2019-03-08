namespace Futilef.V2 {
	public abstract class Drawable {
		public Drawable parent;

		public int anchorAlign = Align.bottomLeft;
		public Vec2 customAnchorAlign;
		public int pivotAlign = Align.bottomLeft;
		public Vec2 customPivotAlign;

		public Vec2 pos;
		public int relativePosAxes = Axes.none;
		public Vec2 size;
		public int relativeSizeAxes = Axes.none;
		public Vec2 scl = new Vec2(1);
		public float rot;

		public Vec4 color = new Vec4(1);
		public float alpha = 1;
		public int srcBlend = BlendFactor.dstColor;
		public int dstBlend = BlendFactor.zero;
		public int blendOp = BlendOperator.add;

		public bool handleInput = true;
		public bool useLayout = true;
		public bool needMatConcatInverse;

		public bool hasTransformChanged = true;
		public bool hasColorChanged = true;

		public Vec2 cachedAnchor;
		public Vec2 cachedPivot;
		public Vec2 cachedPos;
		public Vec2 cachedSize;

		public Mat2D cachedMat;
		public Mat2D cachedMatConcat;
		public Mat2D cachedMatConcatInverse;

		public Vec4 cachedColor;

		readonly DrawNode[] drawNodes = new DrawNode[3];

		public virtual DrawNode GenerateDrawNodeSubtree(int index) {
			var node = drawNodes[index] ?? (drawNodes[index] = CreateDrawNode());
			UpdateDrawNode(node);
			return node;
		}

		protected abstract DrawNode CreateDrawNode();
		protected virtual void UpdateDrawNode(DrawNode node) {}

		public virtual void UpdateTransform() {
			hasTransformChanged = false;

			bool useParentSize = parent != null && parent.useLayout;
			if (useParentSize) {
				cachedPos = Axes.Calc(relativePosAxes, pos, parent.cachedSize);
				cachedSize = Axes.Calc(relativeSizeAxes, size, parent.cachedSize);
			} else {
				cachedPos = Axes.Calc(relativePosAxes, pos);
				cachedSize = Axes.Calc(relativeSizeAxes, size);
			}

			if (useLayout) {
				cachedPivot = cachedSize * Align.Calc(pivotAlign, customPivotAlign);
				cachedMat.FromTranslation(-cachedPivot);
				cachedMat.ScaleRotateTranslate(scl, rot, cachedPos);
			} else {
				cachedMat.FromScalingRotationTranslation(scl, rot, cachedPos);
			}

			if (useParentSize) {
				cachedAnchor = parent.cachedSize * Align.Calc(anchorAlign, customAnchorAlign);
				cachedMat.Translate(cachedAnchor);
			}
				
			cachedMatConcat = parent == null ? cachedMat : parent.cachedMatConcat * cachedMat;
			if (needMatConcatInverse) cachedMatConcatInverse.FromInverse(cachedMatConcat);
		}

		public virtual void UpdateColor() {
			hasColorChanged = false;

			cachedColor = color;
			cachedColor.w *= alpha;

			if (parent != null) {
				cachedColor = Vec4.Blend(cachedColor, parent.cachedColor, srcBlend, dstBlend, blendOp);
			}
		}

		public virtual bool Propagate(UiEvent e) { return e.Trigger(this); }
		public virtual bool OnTouchDown(TouchDownEvent e) { return false; }
		public virtual bool OnTouchMove(TouchMoveEvent e) { return false; }
		public virtual bool OnTouchUp(TouchUpEvent e) { return false; }
		public virtual bool OnTap(TapEvent e) { return false; }
		public virtual bool OnDragStart(DragStartEvent e) { return false; }
		public virtual bool OnDrag(DragEvent e) { return false; }
		public virtual bool OnDragEnd(DragEndEvent e) { return false; }
		public virtual bool OnKeyDown(KeyDownEvent e) { return false; }
		public virtual bool OnKeyUp(KeyUpEvent e) { return false; }
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

		public static Vec2 Calc(int align, Vec2 val) {
			if (align == Align.custom) return val;

			if ((align & Align.left) != 0) val.x = 0;
			else if ((align & Align.centerH) != 0) val.x = .5f;
			else if ((align & Align.right) != 0) val.x = 1;

			if ((align & Align.top) != 0) val.y = 1;
			else if ((align & Align.centerV) != 0) val.y = .5f;
			else if ((align & Align.bottom) != 0) val.y = 0;
			return val;
		}

		public static Vec2 Calc(int align) {
			return Calc(align, new Vec2());
		}
	}

	public static class Axes {
		public const int none = 0;
		public const int x = 1 << 0;
		public const int y = 1 << 1;
		public const int both = x | y;

		public static Vec2 Calc(int relativeAxes, Vec2 val, Vec2 parentVal) {
			if ((relativeAxes & Axes.x) != 0) val.x *= parentVal.x;
			if ((relativeAxes & Axes.y) != 0) val.y *= parentVal.y;
			return val;
		}

		public static Vec2 Calc(int relativeAxes, Vec2 val) {
			if ((relativeAxes & Axes.x) != 0) val.x = 0;
			if ((relativeAxes & Axes.y) != 0) val.y = 0;
			return val;
		}
	}
}

