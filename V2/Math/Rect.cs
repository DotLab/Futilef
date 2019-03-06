namespace Futilef.V2 {
	/**
	 * (x, y + h) - (x + w, y + h)
	 * |            |
	 * (x, y) ----- (x + w, y)
	 */
	public struct Rect {
		public float x;
		public float y;
		public float w;
		public float h;

		public Rect(float w, float h) {
			this.x = 0;
			this.y = 0;
			this.w = w;
			this.h = h;
		}

		public Rect(float x, float y, float w, float h) {
			this.x = x;
			this.y = y;
			this.w = w;
			this.h = h;
		}

		public void Set(float x, float y, float w, float h) {
			this.x = x;
			this.y = y;
			this.w = w;
			this.h = h;
		}
	}
}

