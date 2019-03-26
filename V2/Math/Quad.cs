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

		public void Set(Rect rect) {
			tl.Set(rect.x, rect.y + rect.h);
			tr.Set(rect.x + rect.w, rect.y + rect.h);
			bl.Set(rect.x, rect.y);
			br.Set(rect.x + rect.w, rect.y);
		}

		public void Set(float x, float y, float w, float h) {
			tl.Set(x, y + h);
			tr.Set(x + w, y + h);
			bl.Set(x, y);
			br.Set(x + w, y);
		}

		public Rect GetAabb() {
			float t = bl.y; if (t < br.y) t = br.y; if (t < tl.y) t = tl.y; if (t < tr.y) t = tr.y; 
			float b = bl.y; if (b > br.y) b = br.y; if (b > tl.y) b = tl.y; if (b > tr.y) b = tr.y; 
			float l = bl.x; if (l > br.x) l = br.x; if (l > tl.x) l = tl.x; if (l > tr.x) l = tr.x; 
			float r = bl.x; if (r < br.x) r = br.x; if (r < tl.x) r = tl.x; if (r < tr.x) r = tr.x; 
			return new Rect(l, b, r - l, t - b);
		}

		public static Quad operator +(Quad q, Vec2 v) {
			return new Quad(q.tl + v, q.tr + v, q.bl + v, q.br + v);
		}

		public static Quad operator -(Quad q, Vec2 v) {
			return new Quad(q.tl - v, q.tr - v, q.bl - v, q.br - v);
		}

		public static Quad operator *(Quad q, float f) {
			return new Quad(q.tl * f, q.tr * f, q.bl * f, q.br * f);
		}

		public static Quad operator *(Mat2D m, Quad a) {
			return new Quad(m * a.tl, m * a.tr, m * a.bl, m * a.br);
		}

		public override string ToString() {
			return string.Format("[Quad: tl={0}, tr={1}, bl={2}, br={3}]", tl, tr, bl, br);
		}
	}
}

