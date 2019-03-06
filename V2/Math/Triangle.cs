namespace Futilef.V2 {
	public struct Triangle {
		public Vec2 p0;
		public Vec2 p1;
		public Vec2 p2;

		public bool Contains(Vec2 pos) {
			// This code parametrizes pos as a linear combination of 2 edges s*(p1-p0) + t*(p2->p0).
			// pos is contained if s>0, t>0, s+t<1
			float area2 = p0.y * (p2.x - p1.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y - p1.y * p2.x;
			if (area2 == 0) return false;
			float s = (p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * pos.x + (p0.x - p2.x) * pos.y) / area2;
			if (s < 0) return false;
			float t = (p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * pos.x + (p1.x - p0.x) * pos.y) / area2;
			if (t < 0 || s + t > 1) return false;
			return true;
		}
	}
}

