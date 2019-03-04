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

		public void One() {
			x = 1;
			y = 1;
			z = 1;
			w = 1;
		}
	}
}

