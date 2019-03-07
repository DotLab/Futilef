namespace Futilef.V2 {
	public struct Vec2 {
		public float x;
		public float y;

		public Vec2(float x, float y) {
			this.x = x;
			this.y = y;
		}

		public Vec2(float v) {
			x = v;
			y = v;
		}

		public void Set(float x, float y) {
			this.x = x;
			this.y = y;
		}

		public void Add(Vec2 a) {
			x += a.x;
			y += a.y;
		}

		public void Mult(Vec2 a) {
			x *= a.x;
			y *= a.y;
		}

		public void Mult(float f) {
			x *= f;
			y *= f;
		}

		public static Vec2 operator +(Vec2 v1, Vec2 v2) {
			return new Vec2(v1.x + v2.x, v1.y + v2.y);
		}

		public static Vec2 operator -(Vec2 v) {
			return new Vec2(-v.x, -v.y);
		}

		public static Vec2 operator -(Vec2 v1, Vec2 v2) {
			return new Vec2(v1.x - v2.x, v1.y - v2.y);
		}

		public static Vec2 operator *(Vec2 v1, Vec2 v2) {
			return new Vec2(v1.x * v2.x, v1.y * v2.y);
		}

		/**
		 * m0 m1 m2   x   x * m0 + y * m1 + m2
		 * m3 m4 m5 . y = x * m3 + y * m4 + m5
		 * 0  0  1    1   1
		 */
		public static Vec2 operator *(Mat2D m, Vec2 a) {
			float x = a.x, y = a.y;
			return new Vec2(
				x * m.m0 + y * m.m1 + m.m2,
				x * m.m3 + y * m.m4 + m.m5);
		}

		public override string ToString() {
			return string.Format("({0:F2}, {1:F2})", x, y);
		}
	}
}

