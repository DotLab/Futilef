namespace Futilef.V2 {
	public abstract class DrawNode {
		public uint age;

		public abstract void Draw(DrawCtx ctx, int g);
	}
}
