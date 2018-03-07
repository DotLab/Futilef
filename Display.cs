using System;

using UnityEngine;

namespace Futilef {
	public static class Display {
		public static event Action SignalResize;

		// Set by Init()
		public static float ReferenceLength;
		public static float Pixel2Display, Display2Pixel;
		public static float DisplayScaling, ResourceScaling;

		public static float PixelWidth, PixelHeight;
		public static float Width, Height;
		public static float HalfWidth, HalfHeight;

		static int _intPixelWidth, _intPixelHeight;

		public static void Init(float referenceLength, float displayScaling, float resourceScaling) {
			ReferenceLength = referenceLength;
			DisplayScaling = displayScaling;
			ResourceScaling = resourceScaling;
		}

		static void UpdateScreenDimensions() {
			float screenLongLength = Mathf.Max(Screen.height, Screen.width);
			Display2Pixel = DisplayScaling * (screenLongLength / ReferenceLength);
			Pixel2Display = 1 / Display2Pixel;

			PixelWidth = _intPixelWidth = Screen.width;
			PixelHeight = _intPixelHeight = Screen.height;
			
			Width = PixelWidth * Pixel2Display;
			Height = PixelHeight * Pixel2Display;
			
			HalfWidth = Width / 2;
			HalfHeight = Height / 2;

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

