namespace Futilef.V2.Graphics {
	public abstract class LayoutElement : Drawable{
		public int relPosAxes;

		public Vec2 size;
		public int relSizeAxes;
	}

	public static class Anchor {
		public const int topLeft = top | left;
		public const int topCentre = top | centerH;
		public const int topRight = top | right;

		public const int centreLeft = centerV | left;
		public const int centre = centerV | centerH;
		public const int centreRight = centerV | right;

		public const int bottomLeft = bottom | left;
		public const int bottomCentre = bottom | centerH;
		public const int bottomRight = bottom | right;

		public const int top = 1 << 0;
		public const int centerV = 1 << 1;
		public const int bottom = 1 << 2;

		public const int left = 1 << 3;
		public const int centerH = 1 << 4;
		public const int right = 1 << 5;

		public const int Custom = 1 << 6;
	}

	public static class Axes {
		public const int none = 0;
		public const int x = 1 << 0;
		public const int y = 1 << 1;
		public const int both = x | y;
	}
}

