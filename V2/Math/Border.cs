namespace Futilef.V2 {
	public struct Border {
		public float l;
		public float r;
		public float t;
		public float b;

		public Border(float l, float r, float t, float b) {
			this.l = l;
			this.r = r;
			this.t = t;
			this.b = b;
		}

		public static Border operator *(Border b, float f) {
			return new Border(b.l * f, b.r * f, b.t * f, b.b * f);
		}

		public override string ToString() {
			return string.Format("({0:F2}, {1:F2}, {2:F2}, {3:F2})", l, r, t, b);
		}
	}
}

