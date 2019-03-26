using System.Collections.Generic;
using UnityInput = UnityEngine.Input;

namespace Futilef.V2 {
	public class InputHandler {
		public sealed class TouchState {
			public int id;
			public Vec2 startPos;
			public double startTime;
			public bool isDragging;
		}

		public const int MouseId = -3000;
		public const double TapTime = 0.2;
		public const float MinDragDistance = 10;

		public readonly object eventListMutex = new object();
		public List<UiEvent> eventList = new List<UiEvent>();

		public readonly Dictionary<int, TouchState> touchStateDict = new Dictionary<int, TouchState>();

		public readonly UnityEngine.Camera eventCamera;

		public InputHandler(UnityEngine.Camera eventCamera) {
			this.eventCamera = eventCamera;
		}

		public void ProcessUnityInput(double time) {
			int touchCount = UnityInput.touchCount;
			TouchState ts;

			lock (eventListMutex) {
				for (int i = 0; i < touchCount; i++) {
					var touch = UnityInput.GetTouch(i);
					int id = touch.fingerId;
					var pos = new Vec2(touch.position.x, touch.position.y);

					switch (touch.phase) {
					case UnityEngine.TouchPhase.Began:  // new touch
						if (touchStateDict.TryGetValue(id, out ts)) {
							FinishTouch(ts, pos, time);
						}
						StartTouch(id, pos, time);
						break;
					case UnityEngine.TouchPhase.Moved:
					case UnityEngine.TouchPhase.Stationary:
						if (touchStateDict.TryGetValue(id, out ts)) {
							UpdateTouch(ts, pos, time);
						} else {
							StartTouch(id, pos, time);
						}
						break;
					case UnityEngine.TouchPhase.Ended:
					case UnityEngine.TouchPhase.Canceled:  // touch ended
						if (touchStateDict.TryGetValue(id, out ts)) {
							FinishTouch(ts, pos, time);
						}
						break;
					}
				}
			}

			#if UNITY_EDITOR
			lock (eventListMutex) {
				var pos = new Vec2(UnityInput.mousePosition.x, UnityInput.mousePosition.y);

				if (UnityInput.GetMouseButton(0)) {
					if (touchStateDict.TryGetValue(MouseId, out ts)) {
						UpdateTouch(ts, pos, time);
					} else {
						StartTouch(MouseId, pos, time);
					}
				} else {
					if (touchStateDict.TryGetValue(MouseId, out ts)) {
						FinishTouch(ts, pos, time);
					}
				}
			}
			#endif
		}

		void StartTouch(int id, Vec2 pos, double time) {
			pos = ScreenPosToWorld(pos);
//			UnityEngine.Debug.Log("start touch");
			touchStateDict.Add(id, new TouchState{
				id = id,
				startPos = pos,
				startTime = time,
				isDragging = false,
			});
			eventList.Add(new TouchDownEvent{id = id, pos = pos, liveTime = 0});
		}

		void UpdateTouch(TouchState ts, Vec2 pos, double time) {
			pos = ScreenPosToWorld(pos);
//			UnityEngine.Debug.Log("update touch");
			eventList.Add(new TouchMoveEvent{id = ts.id, pos = pos, startPos = ts.startPos, liveTime = time - ts.startTime});
			if (ts.isDragging) {
				eventList.Add(new DragEvent{id = ts.id, pos = pos, startPos = ts.startPos, liveTime = time - ts.startTime});
			} else {
				float distanceX = ts.startPos.x - pos.x; if (distanceX < 0) distanceX = -distanceX;
				float distanceY = ts.startPos.y - pos.y; if (distanceY < 0) distanceY = -distanceY;
				if (distanceX > MinDragDistance || distanceY > MinDragDistance) {
					ts.isDragging = true;
					eventList.Add(new DragStartEvent{id = ts.id, pos = pos, liveTime = time - ts.startTime});
				}
			}
		}

		void FinishTouch(TouchState ts, Vec2 pos, double time) {
			pos = ScreenPosToWorld(pos);
//			UnityEngine.Debug.Log("finish touch");
			touchStateDict.Remove(ts.id);
			if (ts.isDragging) {
				eventList.Add(new DragEndEvent{id = ts.id, pos = pos, startPos = ts.startPos, liveTime = time - ts.startTime});
			}
			if (time - ts.startTime < TapTime) {
				float distanceX = ts.startPos.x - pos.x; if (distanceX < 0) distanceX = -distanceX;
				float distanceY = ts.startPos.y - pos.y; if (distanceY < 0) distanceY = -distanceY;
				if (distanceX < MinDragDistance && distanceY < MinDragDistance) {
					eventList.Add(new TapEvent{id = ts.id, pos = pos, startPos = ts.startPos, liveTime = time - ts.startTime});
				}
			}
			eventList.Add(new TouchUpEvent{id = ts.id, pos = pos, startPos = ts.startPos, liveTime = time - ts.startTime});
		}

		Vec2 ScreenPosToWorld(Vec2 pos) {
			var p = eventCamera.ScreenToWorldPoint(new UnityEngine.Vector3(pos.x, pos.y, -eventCamera.transform.position.z));
			return new Vec2(p.x, p.y);
		}
	}
}

