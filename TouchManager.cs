using System;
using System.Collections.Generic;

using UnityEngine;

namespace Futilef {

	public sealed class Touch {
		public enum Phase {
			Began,
			Stay,
			Ended,
		}

		public readonly int id;
		public Vector2 position;
		public Phase phase;
		
		public Touch(int id) {
			this.id = id;
		}

		public override string ToString() {
			return string.Format("[Touch: id={0}, position={1}, phase={2}]", id, position, phase);
		}
	}

	public interface ISingleTouchable {
		int TouchPriority{ get; }
		bool OnSingleTouchBegan(Touch touch);
		void OnSingleTouchStay(Touch touch);
		void OnSingleTouchEnded(Touch touch);
	}

	public interface IMultiTouchable {
		void OnMultiTouch(Touch[] touches);
	}

	public static class TouchManager {
		public const int MouseTouchId = -1;

		static readonly Dictionary<int, ISingleTouchable> _activeSingleTouchableById = new Dictionary<int, ISingleTouchable>();

		static readonly List<IMultiTouchable> _multiTouchables = new List<IMultiTouchable>();
		static readonly List<ISingleTouchable> _singleTouchables = new List<ISingleTouchable>();
		static bool _isSingleTouchablesDirty;

		public static void Init() {
			Input.multiTouchEnabled = true;
		}

		public static void AddTouchalbe(ISingleTouchable touchable) {
			if (_singleTouchables.Contains(touchable)) throw new Exception("Duplicated single touchable");
			_singleTouchables.Add(touchable);
			_isSingleTouchablesDirty = true;
		}

		public static void RemoveTouchable(ISingleTouchable touchable) {
			if (!_singleTouchables.Contains(touchable)) throw new Exception("Nonexisting single touchable");
			_singleTouchables.Remove(touchable);
		}

		public static void AddTouchalbe(IMultiTouchable touchable) {
			if (_multiTouchables.Contains(touchable)) throw new Exception("Duplicated multi touchable");
			_multiTouchables.Add(touchable);
		}

		public static void RemoveTouchable(IMultiTouchable touchable) {
			if (!_multiTouchables.Contains(touchable)) throw new Exception("Nonexisting multi touchable");
			_multiTouchables.Remove(touchable);
		}

		public static void OnUpdate() {
			if (_isSingleTouchablesDirty) {
				_isSingleTouchablesDirty = false;
				_singleTouchables.Sort((a, b) => b.TouchPriority - a.TouchPriority);
			}

			// Mouse Touch
			bool isMouseActive = true;
			var mouseTouch = new Touch(-1);

			mouseTouch.position = ((Vector2)Input.mousePosition) * Display.Pixel2Display;

			if (Input.GetMouseButtonDown(0)) mouseTouch.phase = Touch.Phase.Began;
			else if (Input.GetMouseButtonUp(0)) mouseTouch.phase = Touch.Phase.Ended;
			else if (Input.GetMouseButton(0)) mouseTouch.phase = Touch.Phase.Stay;
			else isMouseActive = false;

			// Unity Touches
			var unityTouches = Input.touches;
			var touches = new Touch[unityTouches.Length + (isMouseActive ? 1 : 0)];
			if (isMouseActive) touches[touches.Length - 1] = mouseTouch;

			for (int i = 0; i < unityTouches.Length; i++) {
				var unityTouch = unityTouches[i];
				var touch = new Touch(unityTouch.fingerId);

				touch.position = unityTouch.position * Display.Pixel2Display;

				// Map touch phase
				if (unityTouch.phase == TouchPhase.Began) touch.phase = Touch.Phase.Began;
				else if (unityTouch.phase == TouchPhase.Moved || unityTouch.phase == TouchPhase.Stationary) touch.phase = Touch.Phase.Stay;
				else if (unityTouch.phase == TouchPhase.Ended || unityTouch.phase == TouchPhase.Canceled) touch.phase = Touch.Phase.Ended;

				touches[i] = touch;
			}

			// Single Touch
			foreach (var touch in touches) {
				// End touches
				if (touch.phase == Touch.Phase.Began || touch.phase == Touch.Phase.Ended) {
					if (_activeSingleTouchableById.ContainsKey(touch.id)) {
						_activeSingleTouchableById[touch.id].OnSingleTouchEnded(touch);
						_activeSingleTouchableById.Remove(touch.id);
					}
				}

				if (touch.phase == Touch.Phase.Began) {
					// Find and add mapping
					foreach (var touchable in _singleTouchables) {
						if (touchable.OnSingleTouchBegan(touch)) {
							_activeSingleTouchableById.Add(touch.id, touchable);
							break;
						}
					}
				} else if (touch.phase == Touch.Phase.Stay) {
					// Stay on active touchable
					if (_activeSingleTouchableById.ContainsKey(touch.id)) _activeSingleTouchableById[touch.id].OnSingleTouchStay(touch);
				}

				// Multi Touch
				if (touches.Length > 0) foreach (var touchable in _multiTouchables) touchable.OnMultiTouch(touches);

//				LogTouches(touches);
			}
		}

		public static void LogTouches(Touch[] touches) {
			var sb = new System.Text.StringBuilder();

			foreach (var touch in touches) {
				sb.AppendLine(touch.ToString());
			}

			Debug.Log(sb.ToString());
		}
	}
}

