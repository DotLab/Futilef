using System.Collections.Generic;

namespace Futilef.V2 {
	public class InputManager {
		public readonly UnityInputHandler handler;
		public List<UiEvent> eventList = new List<UiEvent>();

		public Drawable curFocus;

		public InputManager(UnityInputHandler handler) {
			this.handler = handler;
		}

		public void Update(Drawable root) {
			lock (handler.eventListMutex) {
				var temp = handler.eventList;
				handler.eventList = eventList;
				eventList = temp;
			}

			if (root.handleInput) {
				foreach (var e in eventList) {
//					Console.Log("InputManager", root.GetType(), e.GetType());
					if (e is TouchDownEvent) {
						var focus = root.Propagate(e);
						if (curFocus != focus) {
							Console.Log("focus", focus != null ? focus.GetType().ToString() : "null");
							if (curFocus != null) curFocus.Propagate(new FocusLostEvent{});
							curFocus = focus;
						}
					} else if (e is TouchEvent) {
						root.Propagate(e);
					} else {
						if (curFocus != null) curFocus.Propagate(e);
					}
				}
			}

			eventList.Clear();
		}
	}
}

