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

		public Quad(float x, float y, float w, float h) {
			tl = new Vec2(x, y + h);
			tr = new Vec2(x + w, y + h);
			bl = new Vec2(x, y);
			br = new Vec2(x + w, y);
		}

		public Quad(Rect rect) {
			float x = rect.x, y = rect.y, w = rect.w, h = rect.h;
			tl = new Vec2(x, y + h);
			tr = new Vec2(x + w, y + h);
			bl = new Vec2(x, y);
			br = new Vec2(x + w, y);
		}

		public void FromRect(Rect rect) {
			tl.Set(rect.x, rect.y + rect.h);
			tr.Set(rect.x + rect.w, rect.y + rect.h);
			bl.Set(rect.x, rect.y);
			br.Set(rect.x + rect.w, rect.y);
		}

		public void FromRect(float x, float y, float w, float h) {
			tl.Set(x, y + h);
			tr.Set(x + w, y + h);
			bl.Set(x, y);
			br.Set(x + w, y);
		}

		public static Quad operator +(Quad q, Vec2 v) {
			return new Quad(q.tl + v, q.tr + v, q.bl + v, q.br + v);
		}

		public static Quad operator -(Quad q, Vec2 v) {
			return new Quad(q.tl - v, q.tr - v, q.bl - v, q.br - v);
		}

		public static Quad operator *(Mat2D m, Quad a) {
			return new Quad(m * a.tl, m * a.tr, m * a.bl, m * a.br);
		}
	}
}

