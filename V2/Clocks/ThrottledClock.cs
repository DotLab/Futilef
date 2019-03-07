namespace Futilef.V2 {
	public class ThrottledClock : FramedClock {
		public readonly double maxFrameRate;

		public ThrottledClock() {
			maxFrameRate = 200;
		}

		public ThrottledClock(double maxFrameRate) {
			this.maxFrameRate = maxFrameRate;
		}

		public override void NewFrame() {
			base.NewFrame();

			bool shoudlYield = true;

			double targetDelta = 1.0 / maxFrameRate;
			if (delta < targetDelta) {
				int sleepMs = (int)((targetDelta - delta) * 1000.0);
				if (sleepMs > 0) {
					System.Threading.Thread.Sleep(sleepMs);
					shoudlYield = false;

					curTime = source.GetTime();
					delta = curTime - lastTime;
				}
			}

			if (shoudlYield) {
				System.Threading.Thread.Sleep(0);
			}
		}
	}
}

