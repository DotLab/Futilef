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
	}
}

