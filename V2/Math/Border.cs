namespace Futilef.V2 {
	public struct Border {
		public float l;
		public float r;
		public float b;
		public float t;

		public Border(float l, float r, float b, float t) {
			this.l = l;
			this.r = r;
			this.b = b;
			this.t = t;
		}

		public void Set(float l, float r, float b, float t) {
			this.l = l;
			this.r = r;
			this.b = b;
			this.t = t;
		}

		public void Set(float v) {
			l = v;
			r = v;
			b = v;
			t = v;
		}

		public static Border operator *(Border b, float f) {
			return new Border(b.l * f, b.r * f, b.b * f, b.t * f);
		}

		public override string ToString() {
			return string.Format("({0:F2}, {1:F2}, {2:F2}, {3:F2})", l, r, b, t);
		}
	}
}

