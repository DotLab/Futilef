using System;

using UnityEngine;

namespace Futilef {
	public static class FScreen {
		public static float DisplayScale, ResourceScale;
		public static float ScreenPixelOffset;
		public static string ResourceSuffix;

		public static float Width, Height;
		public static float PixelWidth, PixelHeight;

		public static event Action SignalResize;

		public static float OriginX, OriginY;

		static float _screenLongLength, _screenShortLength;

		static public bool _didJustResize;
		static public float _oldWidth, _oldHeight;

		static FResLevel _resLevel;

		static FConfig _config;

		public static void Init(FConfig config) {
			_config = config;


			OriginX = _config.origin.x;
			OriginY = _config.origin.y;

			// Choose resolution level
			if (_config.resLevels.Count < 1) throw new Exception("You must add at least one FResLevel");
			_resLevel = null;
			foreach (var resLevel in config.resLevels) {
				if (_screenLongLength <= resLevel.maxLength) {
					_resLevel = resLevel;
					break;
				}
			}
			if (_resLevel == null) _resLevel = _config.resLevels.GetLastItem();

			DisplayScale = _resLevel.displayScale * _screenLongLength / _resLevel.maxLength;
			ResourceScale = _resLevel.resourceScale;
			ScreenPixelOffset = Futilef.IsOpenGl ? 0 : 0.5f / DisplayScale;

			UpdateScreenDimensions();

			_didJustResize = true;
		}

		public static void OnUpdate() {
			if (_didJustResize) {
				_didJustResize = false;
				_oldWidth = Screen.width;
				_oldHeight = Screen.height;
			} else if (_oldWidth != Screen.width || _oldHeight != Screen.height) {
				_oldWidth = Screen.width;
				_oldHeight = Screen.height;

				UpdateScreenDimensions();
				Futilef.Instance.UpdateCameraPosition();

				if (SignalResize != null) SignalResize();
			}
		}

		public static void UpdateScreenDimensions() {
			_screenLongLength = Math.Max(Screen.height, Screen.width);
			_screenShortLength = Math.Max(Screen.height, Screen.width);

			PixelWidth = _screenLongLength;
			PixelHeight = _screenShortLength;

			Width = PixelWidth / DisplayScale;
			Height = PixelHeight / DisplayScale;
		}
	}
}

