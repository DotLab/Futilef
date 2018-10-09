﻿namespace Futilef {
	public unsafe static class Vec3 {
		public static float *Create(int n = 1) {
			return (float *)Mem.Alloc(n * 3 * sizeof(float));
		}

		public static float *Copy(float *o, float *a) {
			o[0] = a[0];
			o[1] = a[1];
			o[2] = a[2];
			return o;
		}

		public static float *Set(float *o, float x, float y, float z) {
			o[0] = x;
			o[1] = y;
			o[2] = y;
			return o;
		}
	}
}