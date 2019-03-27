namespace Futilef.V2 {
	public class UiEventDelegatingContainer : CompositeDrawable {
		public System.Func<TouchDownEvent, bool> onTouchDown;
		public System.Func<TouchMoveEvent, bool> onTouchMove;
		public System.Func<TouchUpEvent, bool> onTouchUp;
		public System.Func<TapEvent, bool> onTap;
		public System.Func<DragStartEvent, bool> onDragStart;
		public System.Func<DragEvent, bool> onDrag;
		public System.Func<DragEndEvent, bool> onDragEnd;
		public System.Func<KeyDownEvent, bool> onKeyDown;
		public System.Func<KeyUpEvent, bool> onKeyUp;

		public UiEventDelegatingContainer() {
			needScreenAabb = true;
		}

		public override Drawable Propagate(UiEvent e) {
			var focus = PropagateChildren(e);
			if (focus != null) return focus;

			var touchEvent = e as TouchEvent;
			if (touchEvent == null || cachedScreenAabb.Contains(touchEvent.pos)) {
				if (e.Trigger(this)) return this;
			}

			return null;
		}

		public override bool OnTouchDown(TouchDownEvent e) { return onTouchDown != null && onTouchDown(e); }
		public override bool OnTouchMove(TouchMoveEvent e) { return onTouchMove != null && onTouchMove(e); }
		public override bool OnTouchUp(TouchUpEvent e) { return onTouchUp != null && onTouchUp(e); }
		public override bool OnTap(TapEvent e) { return onTap != null && onTap(e); }
		public override bool OnDragStart(DragStartEvent e) { return onDragStart != null && onDragStart(e); }
		public override bool OnDrag(DragEvent e) { return onDrag != null && onDrag(e); }
		public override bool OnDragEnd(DragEndEvent e) { return onDragEnd != null && onDragEnd(e); }
		public override bool OnKeyDown(KeyDownEvent e) { return onKeyDown != null && onKeyDown(e); }
		public override bool OnKeyUp(KeyUpEvent e) { return onKeyUp != null && onKeyUp(e); }
	}
}

