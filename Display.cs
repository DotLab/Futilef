using System;

using UnityEngine;

namespace Futilef {
	public static class Display {
		public static event Action SignalResize;

		// Set by Init()
		public static float ReferenceLength;
		public static float Pixel2Display, Display2Pixel;
		public static float ResourceScaling;

		public static float PixelWidth, PixelHeight;
		public static float Width, Height;
		public static float HalfWidth, HalfHeight;

		static int _iPixelWidth, _iPixelHeight;

		public static void Init(float referenceLength, float displayScaling, float resourceScaling) {
			ReferenceLength = referenceLength;
			ResourceScaling = resourceScaling;

			float screenLongLength = Mathf.Max(Screen.height, Screen.width);
			Display2Pixel = displayScaling * (screenLongLength / referenceLength);
			Pixel2Display = 1 / Display2Pixel;

			UpdateDimensions();
		}

		static void UpdateDimensions() {
			PixelWidth = _iPixelWidth = Screen.width;
			PixelHeight = _iPixelHeight = Screen.height;
			
			Width = PixelWidth * Pixel2Display;
			Height = PixelHeight * Pixel2Display;
			
			HalfWidth = Width / 2;
			HalfHeight = Height / 2;
		}

		public static void OnUpdate() {
			if (_iPixelWidth != Screen.width || _iPixelHeight != Screen.height) {
				UpdateDimensions();

				if (SignalResize != null) SignalResize();
			}
		}
	}
}

