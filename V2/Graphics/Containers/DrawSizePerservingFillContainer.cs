using Math = System.Math;

namespace Futilef.V2 {
	public class DrawSizePerservingFillContainer : CompositeDrawable {
		public static class Strategy {
			public const int minimum = 0;
			public const int maximum = 1;
			public const int average = 2;
			public const int separate = 3;
		}

		public Vec2 targetDrawSize = new Vec2(1024, 768);
		public bool targetDrawSizeDirty;

		public int strategy = Strategy.minimum;

		protected override void UpdateDrawNode(DrawNode node) {
			if (targetDrawSizeDirty) {
				var ratio = cachedSize / targetDrawSize;
				switch (strategy) {
				case Strategy.minimum: scl.Set(Math.Min(ratio.x, ratio.y)); break;
				case Strategy.maximum: scl.Set(Math.Max(ratio.x, ratio.y)); break;
				case Strategy.average: scl.Set(.5f * (ratio.x + ratio.y)); break;
				case Strategy.separate: scl = ratio; break;
				}
				hasTransformChanged = true;
			}

			base.UpdateDrawNode(node);
		}
	}
}

