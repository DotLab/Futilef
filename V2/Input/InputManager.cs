using System.Collections.Generic;

namespace Futilef.V2 {
	public class InputManager {
		public readonly InputHandler handler;
		public List<UiEvent> eventList = new List<UiEvent>();

		public InputManager(InputHandler handler) {
			this.handler = handler;
		}

		public void Update(Drawable root) {
			lock (handler.eventListMutex) {
				var temp = handler.eventList;
				handler.eventList = eventList;
				eventList = temp;
			}

			if (root.handleInput) {
				foreach (var e in eventList) root.Propagate(e);
			}

			eventList.Clear();
		}
	}
}

