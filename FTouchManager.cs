using System;
using System.Collections.Generic;

using UnityEngine;

namespace Futilef {
	public enum FTouchPhase {
		Enter,
		Stay,
		Exit,
	}

	public sealed class FTouch {

		public readonly int id;
		public Vector2 position;
		public FTouchPhase phase;

		public FTouch(int id) {
			this.id = id;
		}

		public override string ToString() {
			return string.Format("[Touch: id={0}, position={1}, phase={2}]", id, position, phase);
		}
	}

	public interface IDepthSortable {
		int depth { get; }
	}

	public interface ISingleTouchable : IDepthSortable {
		bool OnSingleTouchBegan(FTouch touch);
		void OnSingleTouchStay(FTouch touch);
		void OnSingleTouchEnded(FTouch touch);
	}

	public interface IMultiTouchable {
		void OnMultiTouch(FTouch[] touches);
	}

	public static class FTouchManager {
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
				_singleTouchables.Sort((a, b) => b.depth - a.depth);
			}

			// Mouse Touch
			bool isMouseActive = true;
			var mouseTouch = new FTouch(-1);

			mouseTouch.position = ((Vector2)Input.mousePosition) * FScreen.Pixel2Screen;

			if (Input.GetMouseButtonDown(0)) mouseTouch.phase = FTouchPhase.Enter;
			else if (Input.GetMouseButtonUp(0)) mouseTouch.phase = FTouchPhase.Exit;
			else if (Input.GetMouseButton(0)) mouseTouch.phase = FTouchPhase.Stay;
			else isMouseActive = false;

			// Unity Touches
			var unityTouches = Input.touches;
			var touches = new FTouch[unityTouches.Length + (isMouseActive ? 1 : 0)];
			if (isMouseActive) touches[touches.Length - 1] = mouseTouch;

			for (int i = 0; i < unityTouches.Length; i++) {
				var unityTouch = unityTouches[i];
				var touch = new FTouch(unityTouch.fingerId);

				touch.position = unityTouch.position * FScreen.Pixel2Screen;

				// Map touch phase
				if (unityTouch.phase == TouchPhase.Began) touch.phase = FTouchPhase.Enter;
				else if (unityTouch.phase == TouchPhase.Moved || unityTouch.phase == TouchPhase.Stationary) touch.phase = FTouchPhase.Stay;
				else if (unityTouch.phase == TouchPhase.Ended || unityTouch.phase == TouchPhase.Canceled) touch.phase = FTouchPhase.Exit;

				touches[i] = touch;
			}

			// Single Touch
			foreach (var touch in touches) {
				// End touches
				if (touch.phase == FTouchPhase.Enter || touch.phase == FTouchPhase.Exit) {
					if (_activeSingleTouchableById.ContainsKey(touch.id)) {
						_activeSingleTouchableById[touch.id].OnSingleTouchEnded(touch);
						_activeSingleTouchableById.Remove(touch.id);
					}
				}

				if (touch.phase == FTouchPhase.Enter) {
					// Find and add mapping
					foreach (var touchable in _singleTouchables) {
						if (touchable.OnSingleTouchBegan(touch)) {
							_activeSingleTouchableById.Add(touch.id, touchable);
							break;
						}
					}
				} else if (touch.phase == FTouchPhase.Stay) {
					// Stay on active touchable
					if (_activeSingleTouchableById.ContainsKey(touch.id)) _activeSingleTouchableById[touch.id].OnSingleTouchStay(touch);
				}

				// Multi Touch
				if (touches.Length > 0) foreach (var touchable in _multiTouchables) touchable.OnMultiTouch(touches);

				LogTouches(touches);
			}
		}

		public static void LogTouches(FTouch[] touches) {
			var sb = new System.Text.StringBuilder();

			foreach (var touch in touches) {
				sb.AppendLine(touch.ToString());
			}

			Debug.Log(sb.ToString());
		}
	}
}

