using Math = System.Math;

namespace Futilef {
	public unsafe static class Mat2D {
		public static float *New(int n = 1) {
			return (float *)Mem.Malloc(n * 6 * sizeof(float));
		}

		/**
		 * 1 0 x   cos -sin 0   sx 0 0   1 0 0   (sx * cos) (sy * -sin) x
		 * 0 1 y . sin cos  0 . 0 sy 0 . 0 1 0 = (sx * sin) (sy * cos)  y
		 * 0 0 1   0   0    1   0 0  1   0 0 1   0          0           1
		 */
		public static float *FromScalingRotationTranslation(float *o, float *t, float *s, float r) {
			float sin = (float)Math.Sin(r), cos = (float)Math.Cos(r);
			float sx = s[0], sy = s[1], x = t[0], y = t[1];
			o[0] = sx * cos;
			o[1] = sy * -sin;
			o[2] = x;
			o[3] = sx * sin;
			o[4] = sy * cos;
			o[5] = y;
			return o;
		}

		public static string Str(float *o) {
			return string.Format("mat2d({0}, {1}, {2}, {3}, {4}, {5})", o[0], o[1], o[2], o[3], o[4], o[5]);
		}
	}
}