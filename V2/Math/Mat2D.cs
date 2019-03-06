using Math = System.Math;

namespace Futilef.V2 {
	public struct Mat2D {
		public float m0;
		public float m1;
		public float m2;
		public float m3;
		public float m4;
		public float m5;

		public Mat2D(float m0, float m1, float m2, float m3, float m4, float m5) {
			this.m0 = m0;
			this.m1 = m1;
			this.m2 = m2;
			this.m3 = m3;
			this.m4 = m4;
			this.m5 = m5;
		}

		public void FromIdentity() {
			m0 = 1;
			m1 = 0;
			m2 = 0;
			m3 = 1;
			m4 = 0;
			m5 = 0;
		}

		public void FromInverting(Mat2D a) {
			float aa = a.m0, ab = a.m1, ac = a.m2, ad = a.m3;
			float atx = a.m4, aty = a.m5;
			float det = aa * ad - ab * ac;
			if (det == 0) return;
			det = 1f / det;
			m0 = ad * det;
			m1 = -ab * det;
			m2 = -ac * det;
			m3 = aa * det;
			m4 = (ac * aty - ad * atx) * det;
			m5 = (ab * atx - aa * aty) * det;
		}

		/**
		 * 1 0 x   cos -sin 0   sx 0 0   (sx * cos) (sy * -sin) x
		 * 0 1 y . sin cos  0 . 0 sy 0 = (sx * sin) (sy * cos)  y
		 * 0 0 1   0   0    1   0 0  1   0          0           1
		 */
		public void FromScalingRotationTranslation(Vec2 t, float r, Vec2 s) {
			float sx = s.x, sy = s.y;
			float x = t.x, y = t.y;
			float sin = (float)Math.Sin(r), cos = (float)Math.Cos(r);
			m0 = sx * cos;
			m1 = sy * -sin;
			m2 = x;
			m3 = sx * sin;
			m4 = sy * cos;
			m5 = y;
		}

		/**
		 * a * b = o
		 */
		public static Mat2D operator *(Mat2D a, Mat2D b) {
			float a0 = a.m0, a1 = a.m1, a2 = a.m2, a3 = a.m3, a4 = a.m4, a5 = a.m5;
			float b0 = b.m0, b1 = b.m1, b2 = b.m2, b3 = b.m3, b4 = b.m4, b5 = b.m5;
			return new Mat2D(
				a0 * b0 + a2 * b1,
				a1 * b0 + a3 * b1,
				a0 * b2 + a2 * b3,
				a1 * b2 + a3 * b3,
				a0 * b4 + a2 * b5 + a4,
				a1 * b4 + a3 * b5 + a5);
		}
	}
}

