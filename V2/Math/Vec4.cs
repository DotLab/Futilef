namespace Futilef.V2 {
	public struct Vec4 {
		public float x;
		public float y;
		public float z;
		public float w;

		public Vec4(float x, float y, float z, float w) {
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public Vec4(float x) {
			this.x = x;
			this.y = x;
			this.z = x;
			this.w = x;
		}

		public void One() {
			x = 1;
			y = 1;
			z = 1;
			w = 1;
		}

		public static Vec4 operator +(Vec4 a, Vec4 b) {
			return new Vec4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		}

		public static Vec4 operator -(Vec4 a, Vec4 b) {
			return new Vec4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		}

		public static Vec4 operator *(Vec4 a, float f) {
			return new Vec4(a.x * f, a.y * f, a.z * f, a.w * f);
		}
	}
}

