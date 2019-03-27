namespace Futilef.V2 {
	public abstract class UiEvent {
		public abstract bool Trigger(Drawable root);
	}

	public abstract class TouchEvent : UiEvent {
		public int id;
		public Vec2 pos;
		public double liveTime;
	}

	public abstract class KeyEvent : UiEvent {
		public int key;
	}

	public sealed class TouchDownEvent : TouchEvent {
		public override bool Trigger(Drawable root) { return root.handleInput && root.OnTouchDown(this); }
	}

	public sealed class TouchMoveEvent : TouchEvent {
		public Vec2 startPos;
		public override bool Trigger(Drawable root) { return root.handleInput && root.OnTouchMove(this); }
	}

	public sealed class TouchUpEvent : TouchEvent {
		public Vec2 startPos;
		public override bool Trigger(Drawable root) { return root.handleInput && root.OnTouchUp(this); }
	}

	public sealed class TapEvent : TouchEvent {
		public Vec2 startPos;
		public override bool Trigger(Drawable root) { return root.handleInput && root.OnTap(this); }
	}

	public sealed class DragStartEvent : TouchEvent {
		public override bool Trigger(Drawable root) { return root.handleInput && root.OnDragStart(this); }
	}

	public sealed class DragEvent : TouchEvent {
		public Vec2 startPos;
		public override bool Trigger(Drawable root) { return root.handleInput && root.OnDrag(this); }
	}

	public sealed class DragEndEvent : TouchEvent {
		public Vec2 startPos;
		public override bool Trigger(Drawable root) { return root.handleInput && root.OnDragEnd(this); }
	}

	public sealed class KeyDownEvent : KeyEvent {
		public override bool Trigger(Drawable root) { return root.handleInput && root.OnKeyDown(this); }
	}

	public sealed class KeyUpEvent : KeyEvent {
		public override bool Trigger(Drawable root) { return root.handleInput && root.OnKeyUp(this); }
	}

	public sealed class TextInputEvent : UiEvent {
		public string text;
		public override bool Trigger(Drawable root) { return root.handleInput && root.OnTextInput(this); }
	}
}

