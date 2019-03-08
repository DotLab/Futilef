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

		public void FromTranslation(Vec2 t) {
			m0 = 1;
			m1 = 0;
			m2 = t.x;
			m3 = 0;
			m4 = 1;
			m5 = t.y;
		}

		/**
		 * 1 0 x   m0 m1 m2   m0 m1 (m2 + x)
		 * 0 1 y . m3 m4 m5 = m3 m4 (m5 + y)
		 * 0 0 1   0  0  1    0  0  1
		 */
		public void Translate(Vec2 t) {
			m2 += t.x;
			m5 += t.y;
		}

		public void FromIdentity() {
			m0 = 1;
			m1 = 0;
			m2 = 0;
			m3 = 0;
			m4 = 1;
			m5 = 0;
		}

		public void FromInverse(Mat2D a) {
			float aa = a.m0, ab = a.m1, ac = a.m3, ad = a.m4;
			float atx = a.m2, aty = a.m5;
			float det = aa * ad - ab * ac;
			if (det == 0) return;
			det = 1f / det;
			m0 = ad * det;
			m1 = -ab * det;
			m2 = (ac * aty - ad * atx) * det;
			m3 = -ac * det;
			m4 = aa * det;
			m5 = (ab * atx - aa * aty) * det;
		}

		/**
		 * 1 0 x   cos -sin 0   sx 0 0   (sx * cos) (sy * -sin) x
		 * 0 1 y . sin cos  0 . 0 sy 0 = (sx * sin) (sy * cos)  y
		 * 0 0 1   0   0    1   0 0  1   0          0           1
		 */
		public void FromScalingRotationTranslation(Vec2 s, float r, Vec2 t) {
			float sx = s.x, sy = s.y;
			float sin = (float)Math.Sin(r), cos = (float)Math.Cos(r);
			float x = t.x, y = t.y;
			m0 = sx * cos;
			m1 = sy * -sin;
			m2 = x;
			m3 = sx * sin;
			m4 = sy * cos;
			m5 = y;
		}

		/**
		 * sxcos sysin x   m0 m1 m2
		 * sxsin sycos y . m3 m4 m5
		 * 0     0     1   0  0  1
		 */
		public void ScaleRotateTranslate(Vec2 s, float r, Vec2 t) {
			float sx = s.x, sy = s.y;
			float sin = (float)Math.Sin(r), cos = (float)Math.Cos(r);
			float a2 = t.x, a5 = t.y;
			float b0 = m0, b1 = m1, b2 = m2, b3 = m3, b4 = m4, b5 = m5;
			float a0 = sx * cos;
			float a1 = sy * -sin;
			float a3 = sx * sin;
			float a4 = sy * cos;
			m0 = a0 * b0 + a1 * b3; 
			m1 = a0 * b1 + a1 * b4; 
			m2 = a0 * b2 + a1 * b5 + a2;
			m3 = a3 * b0 + a4 * b3; 
			m4 = a3 * b1 + a4 * b4; 
			m5 = a3 * b2 + a4 * b5 + a5;
		}

		/**
		 * a0 a1 a2   b0 b1 b2   (a0 * b0 + a1 * b3) (a0 * b1 + a1 * b4) (a0 * b2 + a1 * b5 + a2)      
		 * a3 a4 a5 . b3 b4 b5 = (a3 * b0 + a4 * b3) (a3 * b1 + a4 * b4) (a3 * b2 + a4 * b5 + a5)
		 * 0  0  1    0  0  1    0                   0                   1
		 */
		public static Mat2D operator *(Mat2D a, Mat2D b) {
			float a0 = a.m0, a1 = a.m1, a2 = a.m2, a3 = a.m3, a4 = a.m4, a5 = a.m5;
			float b0 = b.m0, b1 = b.m1, b2 = b.m2, b3 = b.m3, b4 = b.m4, b5 = b.m5;
			return new Mat2D(
				(a0 * b0 + a1 * b3), (a0 * b1 + a1 * b4), (a0 * b2 + a1 * b5 + a2),
				(a3 * b0 + a4 * b3), (a3 * b1 + a4 * b4), (a3 * b2 + a4 * b5 + a5));
		}

		public override string ToString() {
			return string.Format("({0:F2}, {1:F2}, {2:F2},\n {3:F2}, {4:F2}, {5:F2})", m0, m1, m2, m3, m4, m5);
		}
	}
}

