﻿namespace Futilef.V2 {
	public abstract class Drawable {
		public Drawable parent;

		public int anchorAlign = Align.BottomLeft;
		public Vec2 customAnchorAlign;
		public int pivotAlign = Align.BottomLeft;
		public Vec2 customPivotAlign;

		public Vec2 pos;
		public int relativePosAxes = Axes.None;
		public Vec2 size = new Vec2(1);
		public int relativeSizeAxes = Axes.None;
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

		public bool transformDirty = true;
		public bool colorDirty = true;

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
			transformDirty = false;

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
			colorDirty = false;

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

	public static class DrawableExtension {
		public static T Layout<T> (this T self, float posX, float posY, float sizeX, float sizeY) where T : Drawable {
			self.pos.Set(posX, posY); self.size.Set(sizeX, sizeY); 
			self.transformDirty = true; return self;
		}

		public static T Layout<T> (this T self, float posX, float posY, int relativeSizeAxes, float sizeX, float sizeY) where T : Drawable {
			self.pos.Set(posX, posY); self.relativeSizeAxes = relativeSizeAxes; self.size.Set(sizeX, sizeY); 
			self.transformDirty = true; return self;
		}

		public static T Anchor<T> (this T self, int align, float x, float y) where T : Drawable {
			self.anchorAlign = align; self.customAnchorAlign.Set(x, y); 
			self.transformDirty = true; return self;
		}

		public static T Pivot<T> (this T self, int align, float x, float y) where T : Drawable {
			self.pivotAlign = align; self.customPivotAlign.Set(x, y); 
			self.transformDirty = true; return self;
		}
	}

	public static class Align {
		public const int None = 0;

		public const int TopLeft = Top | Left;
		public const int TopCenter = Top | CenterH;
		public const int TopRight = Top | Right;

		public const int CenterLeft = CenterV | Left;
		public const int Center = CenterV | CenterH;
		public const int centerRight = CenterV | Right;

		public const int BottomLeft = Bottom | Left;
		public const int BottomCentre = Bottom | CenterH;
		public const int BottomRight = Bottom | Right;

		public const int Top = 1 << 0;
		public const int CenterV = 1 << 1;
		public const int Bottom = 1 << 2;

		public const int Left = 1 << 3;
		public const int CenterH = 1 << 4;
		public const int Right = 1 << 5;

		public const int Custom = 1 << 6;

		public static Vec2 Calc(int align, Vec2 val) {
			if (align == Align.Custom) return val;

			if ((align & Align.Left) != 0) val.x = 0;
			else if ((align & Align.CenterH) != 0) val.x = .5f;
			else if ((align & Align.Right) != 0) val.x = 1;

			if ((align & Align.Top) != 0) val.y = 1;
			else if ((align & Align.CenterV) != 0) val.y = .5f;
			else if ((align & Align.Bottom) != 0) val.y = 0;
			return val;
		}

		public static Vec2 Calc(int align) {
			return Calc(align, new Vec2());
		}
	}

	public static class Axes {
		public const int None = 0;
		public const int X = 1 << 0;
		public const int Y = 1 << 1;
		public const int Both = X | Y;

		public static Vec2 Calc(int relativeAxes, Vec2 val, Vec2 parentVal) {
			if ((relativeAxes & Axes.X) != 0) val.x *= parentVal.x;
			if ((relativeAxes & Axes.Y) != 0) val.y *= parentVal.y;
			return val;
		}

		public static Vec2 Calc(int relativeAxes, Vec2 val) {
			if ((relativeAxes & Axes.X) != 0) val.x = 0;
			if ((relativeAxes & Axes.Y) != 0) val.y = 0;
			return val;
		}
	}
}

