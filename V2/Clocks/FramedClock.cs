namespace Futilef.V2 {
	public class FramedClock : IClock {
		public readonly IClock source;

		public double time;
		public double lastTime;
		public double deltaTime;

		public FramedClock() {
			source = new StopwatchClock();
		}

		public FramedClock(IClock source) {
			this.source = source;
		}

		public virtual void NewFrame() {
			lastTime = time;
			time = source.GetTime();
			deltaTime = time - lastTime;
		}

		public double GetTime() {
			return time;
		}

		public void SetRate(double rate) {
			source.SetRate(rate);
		}
	}
}

