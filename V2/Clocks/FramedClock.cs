namespace Futilef.V2 {
	public class FramedClock : IClock {
		public readonly IClock source;

		public double curTime;
		public double lastTime;
		public double delta;

		public FramedClock() {
			source = new StopwatchClock();
		}

		public FramedClock(IClock source) {
			this.source = source;
		}

		public virtual void NewFrame() {
			lastTime = curTime;
			curTime = source.GetTime();
			delta = curTime - lastTime;
		}

		public double GetTime() {
			return curTime;
		}

		public void SetRate(double rate) {
			source.SetRate(rate);
		}
	}
}

