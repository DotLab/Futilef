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

		public Quad(Vec2 tl, Vec2 tr, Vec2 bl, Vec2 br) {
			this.tl = tl;
			this.tr = tr;
			this.bl = bl;
			this.br = br;
		}

		public Quad(Rect rect) {
			tl = new Vec2(rect.x, rect.y + rect.h);
			tr = new Vec2(rect.x + rect.w, rect.y + rect.h);
			bl = new Vec2(rect.x, rect.y);
			br = new Vec2(rect.x + rect.w, rect.y);
		}

		public void Set(float top, float right, float bottom, float left) {
			tl.Set(left, top);
			tr.Set(right, top);
			bl.Set(left, bottom);
			br.Set(right, bottom);
		}

		public void FromRect(Rect rect) {
			tl.Set(rect.x, rect.y + rect.h);
			tr.Set(rect.x + rect.w, rect.y + rect.h);
			bl.Set(rect.x, rect.y);
			br.Set(rect.x + rect.w, rect.y);
		}

		public static Quad operator +(Quad q, Vec2 v) {
			return new Quad(q.tl + v, q.tr + v, q.bl + v, q.br + v);
		}
	}
}

