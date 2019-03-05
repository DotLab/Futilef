namespace Futilef.V2 {
	public abstract class DrawNode {
		public uint age;

		// this is called in the main thread
		public virtual void Draw(DrawCtx ctx, int g) {
		}
	}
}

