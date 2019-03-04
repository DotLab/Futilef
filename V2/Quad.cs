namespace Futilef.V2 {
	/**
	 * topLeft ---- topRight
	 * |            |
	 * bottomLeft - bottomRight
	 */
	public struct Quad {
		public Vec2 tl;
		public Vec2 tr;
		public Vec2 bl;
		public Vec2 br;

		public void FromRect(Rect rect) {
			tl.Set(rect.x, rect.y + rect.h);
			tr.Set(rect.x + rect.w, rect.y + rect.h);
			bl.Set(rect.x, rect.y);
			br.Set(rect.x + rect.w, rect.y);
		}

		public void Set(float top, float right, float bottom, float left) {
			tl.Set(left, top);
			tr.Set(right, top);
			bl.Set(left, bottom);
			br.Set(right, bottom);
		}
	}
}

