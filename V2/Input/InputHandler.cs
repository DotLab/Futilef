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

		readonly int[] keys = {
			0, 8, 9, 12, 13, 19, 27, 32, 33, 34, 35, 36, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 127, 
			256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 315, 316, 317, 318, 319, 
			323, 324, 325, 326, 327, 328, 329, 		
			330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 
		};

		readonly bool[] keyStates;

		public InputHandler(UnityEngine.Camera eventCamera) {
			this.eventCamera = eventCamera;

			keyStates = new bool[keys.Length];
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

				for (int i = 0, end = keys.Length; i < end; i++) {
					int key = keys[i];
					if (UnityInput.GetKey((UnityEngine.KeyCode)key)) {
						if (!keyStates[i]) {
							keyStates[i] = true;
							eventList.Add(new KeyDownEvent{key = key});
						}
					} else {
						if (keyStates[i]) {
							keyStates[i] = false;
							eventList.Add(new KeyUpEvent{key = key});
						}
					}
				}

				string text = UnityInput.inputString;
				if (!string.IsNullOrEmpty(text)) {
					eventList.Add(new TextInputEvent{text = text});
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

	public static class Key {
		public const int None = 0;
		public const int Backspace = 8;
		public const int Tab = 9;
		public const int Clear = 12;
		public const int Return = 13;
		public const int Pause = 19;
		public const int Escape = 27;
		public const int Space = 32;
		public const int Exclaim = 33;
		public const int DoubleQuote = 34;
		public const int Hash = 35;
		public const int Dollar = 36;
		public const int Ampersand = 38;
		public const int Quote = 39;
		public const int LeftParen = 40;
		public const int RightParen = 41;
		public const int Asterisk = 42;
		public const int Plus = 43;
		public const int Comma = 44;
		public const int Minus = 45;
		public const int Period = 46;
		public const int Slash = 47;
		public const int Alpha0 = 48;
		public const int Alpha1 = 49;
		public const int Alpha2 = 50;
		public const int Alpha3 = 51;
		public const int Alpha4 = 52;
		public const int Alpha5 = 53;
		public const int Alpha6 = 54;
		public const int Alpha7 = 55;
		public const int Alpha8 = 56;
		public const int Alpha9 = 57;
		public const int Colon = 58;
		public const int Semicolon = 59;
		public const int Less = 60;
		public const int Equal = 61;
		public const int Greater = 62;
		public const int Question = 63;
		public const int At = 64;
		public const int LeftBracket = 91;
		public const int Backslash = 92;
		public const int RightBracket = 93;
		public const int Caret = 94;
		public const int Underscore = 95;
		public const int BackQuote = 96;
		public const int A = 97;
		public const int B = 98;
		public const int C = 99;
		public const int D = 100;
		public const int E = 101;
		public const int F = 102;
		public const int G = 103;
		public const int H = 104;
		public const int I = 105;
		public const int J = 106;
		public const int K = 107;
		public const int L = 108;
		public const int M = 109;
		public const int N = 110;
		public const int O = 111;
		public const int P = 112;
		public const int Q = 113;
		public const int R = 114;
		public const int S = 115;
		public const int T = 116;
		public const int U = 117;
		public const int V = 118;
		public const int W = 119;
		public const int X = 120;
		public const int Y = 121;
		public const int Z = 122;
		public const int Delete = 127;

		public const int Keypad0 = 256;
		public const int Keypad1 = 257;
		public const int Keypad2 = 258;
		public const int Keypad3 = 259;
		public const int Keypad4 = 260;
		public const int Keypad5 = 261;
		public const int Keypad6 = 262;
		public const int Keypad7 = 263;
		public const int Keypad8 = 264;
		public const int Keypad9 = 265;
		public const int KeypadPeriod = 266;
		public const int KeypadDivide = 267;
		public const int KeypadMultiply = 268;
		public const int KeypadMinus = 269;
		public const int KeypadPlus = 270;
		public const int KeypadEnter = 271;
		public const int KeypadEquals = 272;
		public const int UpArrow = 273;
		public const int DownArrow = 274;
		public const int RightArrow = 275;
		public const int LeftArrow = 276;
		public const int Insert = 277;
		public const int Home = 278;
		public const int End = 279;
		public const int PageUp = 280;
		public const int PageDown = 281;
		public const int F1 = 282;
		public const int F2 = 283;
		public const int F3 = 284;
		public const int F4 = 285;
		public const int F5 = 286;
		public const int F6 = 287;
		public const int F7 = 288;
		public const int F8 = 289;
		public const int F9 = 290;
		public const int F10 = 291;
		public const int F11 = 292;
		public const int F12 = 293;
		public const int F13 = 294;
		public const int F14 = 295;
		public const int F15 = 296;
		public const int Numlock = 300;
		public const int CapsLock = 301;
		public const int ScrollLock = 302;
		public const int RightShift = 303;
		public const int LeftShift = 304;
		public const int RightControl = 305;
		public const int LeftControl = 306;
		public const int RightAlt = 307;
		public const int LeftAlt = 308;
		public const int RightApple = 309;
		public const int LeftCommand = 310;
		public const int LeftWindows = 311;
		public const int RightWindows = 312;
		public const int AltGr = 313;
		public const int Help = 315;
		public const int Print = 316;
		public const int SysReq = 317;
		public const int Break = 318;
		public const int Menu = 319;

		public const int Mouse0 = 323;
		public const int Mouse1 = 324;
		public const int Mouse2 = 325;
		public const int Mouse3 = 326;
		public const int Mouse4 = 327;
		public const int Mouse5 = 328;
		public const int Mouse6 = 329;
		
		public const int JoystickButton0 = 330;
		public const int JoystickButton1 = 331;
		public const int JoystickButton2 = 332;
		public const int JoystickButton3 = 333;
		public const int JoystickButton4 = 334;
		public const int JoystickButton5 = 335;
		public const int JoystickButton6 = 336;
		public const int JoystickButton7 = 337;
		public const int JoystickButton8 = 338;
		public const int JoystickButton9 = 339;
		public const int JoystickButton10 = 340;
		public const int JoystickButton11 = 341;
		public const int JoystickButton12 = 342;
		public const int JoystickButton13 = 343;
		public const int JoystickButton14 = 344;
		public const int JoystickButton15 = 345;
		public const int JoystickButton16 = 346;
		public const int JoystickButton17 = 347;
		public const int JoystickButton18 = 348;
		public const int JoystickButton19 = 349;
	}
}

