using System;
using System.Collections.Generic;

using UnityEngine;

namespace Futilef {
	public class DelayedCallback {
		public Action callback;
		public float timeDelayed;

		public DelayedCallback(Action callback, float timeDelayed) {
			this.callback = callback;
			this.timeDelayed = timeDelayed;
		}
	}

	public class Futilef : MonoBehaviour {
		public static Futilef Instance;

		// Set in Awake
		public static bool IsOpenGl;
		public static int BaseRenderQueueDepth = 3000;
		public static bool ShouldRemoveAtlasElementFileExtensions = true;

		public static FAtlasElement whiteElement;
		public static Color32 white = new Color32(255, 255, 255, 255);

		internal static int nextRenderLayerDepth;

		static List<object> _stages;

		public event Action SignalUpdate;
		public event Action SignalAfterUpdate;
		public event Action SignalLateUpdate;
		public event Action SignalFixedUpdate;

		GameObject _cameraGo;
		Camera _camera;

		bool _shouldRunGcNextUpdate;


		void Awake() {
			Instance = this;
			IsOpenGl = SystemInfo.graphicsDeviceVersion.Contains("OpenGL");
			enabled = false;
		}

		public void Init(FConfig config) {
			enabled = true;

			Application.targetFrameRate = config.targetFrameRate;

			// Global setup
			FShader.Init();
			FFacetType.Init();
			FScreen.Init(config);

			// Camera setup
			_cameraGo = new GameObject();
			_cameraGo.transform.parent = transform;

			_camera = _cameraGo.AddComponent<Camera>();
			_camera.tag = "MainCamera";
			_camera.name = "Camera";
			_camera.clearFlags = CameraClearFlags.SolidColor;
			_camera.backgroundColor = config.backgroundColor;
			_camera.nearClipPlane = 0;
			_camera.farClipPlane = 500;
			_camera.depth = 100;
			_camera.rect = new Rect(0, 0, 1, 1);
			_camera.orthographic = true;
			// _camera.orthographicSize = screen

			UpdateCameraPosition();

			FTouchManager.Init();
			FAtlasManager.Init();

			// Create defualt atlas
			var whiteTex = new Texture2D(2, 2, TextureFormat.RGB24, false);
			whiteTex.SetPixels32(new [] { new Color32(255, 255, 255, 255) });
			whiteTex.Apply();

			FAtlasManager.LoadAtlas("Futile_White", whiteTex);
			whiteElement = FAtlasManager.GetElement("Futile_White");
		}

		public void UpdateCameraPosition() {
			_camera.orthographicSize = FScreen.PixelHeight / 2 / FScreen.DisplayScale;
			_camera.transform.position = new Vector3(
				((FScreen.OriginX - 0.5f) * -FScreen.PixelWidth) / FScreen.DisplayScale + FScreen.ScreenPixelOffset,
				((FScreen.OriginX - 0.5f) * -FScreen.PixelWidth) / FScreen.DisplayScale + FScreen.ScreenPixelOffset,
				-10.0f);
		}

		void Update() {
			if (SignalUpdate != null) SignalUpdate();
			if (SignalAfterUpdate != null) SignalAfterUpdate();

			if (_shouldRunGcNextUpdate) {
				_shouldRunGcNextUpdate = false;
				GC.Collect();
			}
		}

		void LateUpdate() {
			nextRenderLayerDepth = 0;

			if (SignalLateUpdate != null) SignalLateUpdate();
		}

		void FixedUpdate() {
			if (SignalFixedUpdate != null) SignalFixedUpdate();
		}
	}
}