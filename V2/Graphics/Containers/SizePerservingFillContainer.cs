using Math = System.Math;

namespace Futilef.V2 {
	public class SizePerservingFillContainer : CompositeDrawable {
		public static class Strategy {
			public const int Minimum = 0;
			public const int Maximum = 1;
			public const int Average = 2;
			public const int Separate = 3;
		}

		public Vec2 targetSize = new Vec2(800, 600);
		public int strategy = Strategy.Minimum;

		readonly CompositeDrawable content;

		public SizePerservingFillContainer() {
			children.Add(content = new CompositeDrawable());
			content
				.Layout(0, 0, targetSize.x, targetSize.y)
				.Anchor(Align.Center, 0, 0)
				.Pivot(Align.Center, 0, 0);
		}

		public override void Add(Drawable child) {
			content.Add(child);
			age += 1;
		}

		public override void UpdateTransform() {
			base.UpdateTransform();

			var ratio = cachedSize / targetSize;
			switch (strategy) {
			case Strategy.Minimum: content.scl.Set(Math.Min(ratio.x, ratio.y)); break;
			case Strategy.Maximum: content.scl.Set(Math.Max(ratio.x, ratio.y)); break;
			case Strategy.Average: content.scl.Set(.5f * (ratio.x + ratio.y)); break;
			case Strategy.Separate: content.scl = ratio; break;
			}
			content.size = cachedSize / content.scl;
			content.UpdateTransform();
		}
	}
}

