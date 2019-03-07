using System.Diagnostics;

namespace Futilef.V2 {
	public class StopwatchClock : Stopwatch, IClock {
		public readonly double secoundsPerTick;

		public double rate = 1;
		public double lastRateTime;
		public long lastRateTick;

		public StopwatchClock() {
			secoundsPerTick = 1.0 / Frequency;
			Start();
		}

		public double GetTime() {
			return ((ElapsedTicks - lastRateTick) * secoundsPerTick) * rate + lastRateTime;
		}

		public void SetRate(double rate) {
			lastRateTime = GetTime();
			lastRateTick = ElapsedTicks;

			this.rate = rate;
		}
	}
}