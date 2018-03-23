using System;

using UnityEngine;

namespace Futilef {
	public static class FScreen {
		public static event Action SignalResize;

		// Set by Init()
		public static float ReferenceLength, ScreenScaling;

		public static float Pixel2Screen, Screen2Pixel;

		public static float PixelWidth, PixelHeight;
		public static float Width, Height;
		public static float HalfWidth, HalfHeight;

		static int _intPixelWidth, _intPixelHeight;

		public static void Init(float referenceLength, float screenScaling) {
			ReferenceLength = referenceLength;
			ScreenScaling = screenScaling;

			OnUpdate();
		}

		static void UpdateScreenDimensions() {
			float screenLongLength = Mathf.Max(Screen.width, Screen.height);
			Screen2Pixel = Mathf.Lerp(1f, screenLongLength / ReferenceLength, ScreenScaling);
			Pixel2Screen = 1f / Screen2Pixel;

			PixelWidth = _intPixelWidth = Screen.width;
			PixelHeight = _intPixelHeight = Screen.height;

			Width = PixelWidth * Pixel2Screen;
			Height = PixelHeight * Pixel2Screen;

			HalfWidth = Width / 2f;
			HalfHeight = Height / 2f;

			Debug.LogFormat("PixelWidth={0}, PixelHeight={1}, Width={2}, Height={3}", PixelWidth, PixelHeight, Width, Height);
		}

		public static void OnUpdate() {
			if (_intPixelWidth != Screen.width || _intPixelHeight != Screen.height) {
				UpdateScreenDimensions();

				if (SignalResize != null) SignalResize();
			}
		}
	}
}

