namespace Futilef.V2 {
	public struct Vec2 {
		public float x;
		public float y;

		public Vec2(float x, float y) {
			this.x = x;
			this.y = y;
		}

		public void Set(float x, float y) {
			this.x = x;
			this.y = y;
		}

		public static Vec2 operator +(Vec2 v1, Vec2 v2) {
			return new Vec2(v1.x + v2.x, v1.y + v2.y);
		}

		/**
		 * m00 m01 m02   x   x * m00 + y * m01 + m02
		 * m10 m11 m12 . y = x * m10 + y * m11 + m12
		 *  0   0   1    1   1
		 */
		public static Vec2 operator *(Mat2D m, Vec2 a) {
			float x = a.x, y = a.y;
			return new Vec2(
				x * m.m0 + y * m.m1 + m.m2,
				x * m.m3 + y * m.m4 + m.m5);
		}
	}
}

