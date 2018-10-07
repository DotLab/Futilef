namespace Futilef {
	public unsafe static class Vec2 {
		public static float *Create(int n = 1) {
			return (float *)Mem.Alloc(n * 2 * sizeof(float));
		}

		public static void Free(float *a) {
			Mem.Free(a);
		}

		public static float *Copy(float *o, float *a) {
			o[0] = a[0];
			o[1] = a[1];
			return o;
		}

		public static float *Set(float *o, float x, float y) {
			o[0] = x;
			o[1] = y;
			return o;
		}
	}
}