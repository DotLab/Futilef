using System;
using System.Collections.Generic;

using UnityEngine;

namespace Futilef {
	public enum FTouchPhase {
		Began,
		Stay,
		Ended,
	}

	public class FTouch {
		public readonly int id;
		public Vector2 position;
		public FTouchPhase phase;

		public FTouch(int id) {
			this.id = id;
		}
	}

	public interface FISingleTouchable {
		int TouchPriority { get; }
		bool OnSingleTouchBegan(FTouch touch);
		void OnSingleTouchStay(FTouch touch);
		void OnSingleTouchEnded(FTouch touch);
	}

	public interface FIMultiTouchable {
		void OnMultiTouch(FTouch[] touches);
	}

	public static class FTouchManager {
		public const int MouseTouchId = -1;

		static readonly Dictionary<int, FISingleTouchable> _activeSingleTouchableMapping = new Dictionary<int, FISingleTouchable>();

		static readonly List<FIMultiTouchable> _multiTouchables = new List<FIMultiTouchable>();
		static readonly List<FISingleTouchable> _singleTouchables = new List<FISingleTouchable>();
		static bool _isSingleTouchablesDirty;

		public static void Init() {
			Input.multiTouchEnabled = true;
		}

		public static void OnUpdate() {
			if (_isSingleTouchablesDirty) {
				_isSingleTouchablesDirty = false;
				_singleTouchables.Sort((a, b) => b.TouchPriority - a.TouchPriority);
			}

			float offsetX = -FScreen.OriginX / FScreen.PixelWidth;
			float offsetY = -FScreen.OriginY / FScreen.PixelHeight;

			// Mouse Touch
			bool isMouseActive = true;
			var mouseTouch = new FTouch(-1);

			mouseTouch.position = new Vector2(
				(Input.mousePosition.x + offsetX) / FScreen.DisplayScale,
				(Input.mousePosition.y + offsetY) / FScreen.DisplayScale);

			if (Input.GetMouseButtonDown(0)) mouseTouch.phase = FTouchPhase.Began;
			else if (Input.GetMouseButtonUp(0)) mouseTouch.phase = FTouchPhase.Ended;
			else if (Input.GetMouseButton(0)) mouseTouch.phase = FTouchPhase.Stay;
			else isMouseActive = false;

			// Unity Touches
			var unityTouches = Input.touches;
			var touches = new FTouch[unityTouches.Length + (isMouseActive ? 1 : 0)];
			if (isMouseActive) touches[touches.Length - 1] = mouseTouch;

			for (int i = 0; i < unityTouches.Length; i++) {
				var unityTouch = unityTouches[i];
				var touch = new FTouch(unityTouch.fingerId);

				touch.position = new Vector2(
					(unityTouch.position.x + offsetX) / FScreen.DisplayScale,
					(unityTouch.position.y + offsetY) / FScreen.DisplayScale);

				// Map touch phase
				if (unityTouch.phase == TouchPhase.Began) touch.phase = FTouchPhase.Began;
				else if (unityTouch.phase == TouchPhase.Moved || unityTouch.phase == TouchPhase.Stationary) touch.phase = FTouchPhase.Stay;
				else if (unityTouch.phase == TouchPhase.Ended || unityTouch.phase == TouchPhase.Canceled) touch.phase = FTouchPhase.Ended;

				touches[i] = touch;
			}

			// Single Touch
			foreach (var touch in touches) {
				// End touches
				if (touch.phase == FTouchPhase.Began || touch.phase == FTouchPhase.Ended) {
					if (_activeSingleTouchableMapping.ContainsKey(touch.id)) {
						_activeSingleTouchableMapping[touch.id].OnSingleTouchEnded(touch);
						_activeSingleTouchableMapping.Remove(touch.id);
					}
				}

				if (touch.phase == FTouchPhase.Began) {
					// Find and add mapping
					foreach (var touchable in _singleTouchables) {
						if (touchable.OnSingleTouchBegan(touch)) {
							_activeSingleTouchableMapping.Add(touch.id, touchable);
							break;
						}
					}
				} else if (touch.phase == FTouchPhase.Stay) {
					// Stay on active touchable
					if (_activeSingleTouchableMapping.ContainsKey(touch.id)) _activeSingleTouchableMapping[touch.id].OnSingleTouchStay(touch);
				}
			}

			// Multi Touch
			if (touches.Length > 0) foreach (var touchable in _multiTouchables) touchable.OnMultiTouch(touches);
		}

		public static void AddTouchalbe(FISingleTouchable touchable) {
			if (_singleTouchables.Contains(touchable)) throw new Exception("Duplicated single touchable");
			_singleTouchables.Add(touchable);
			_isSingleTouchablesDirty = true;
		}

		public static void RemovTouchable(FISingleTouchable touchable) {
			if (!_singleTouchables.Contains(touchable)) throw new Exception("Nonexisting single touchable");
			_singleTouchables.Remove(touchable);
		}

		public static void AddTouchalbe(FIMultiTouchable touchable) {
			if (_multiTouchables.Contains(touchable)) throw new Exception("Duplicated multi touchable");
			_multiTouchables.Add(touchable);
		}

		public static void RemoveTouchable(FIMultiTouchable touchable) {
			if (!_multiTouchables.Contains(touchable)) throw new Exception("Nonexisting multi touchable");
			_multiTouchables.Remove(touchable);
		}
	}
}

