using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Futilef {
	public sealed class FutilefBehaviour : MonoBehaviour {
		const float CameraOffsetZ = -10;

		public static event Action SignalPreUpdate, SignalUpdate, SignalAfterUpdate;
		public static event Action SignalAfterDraw;
		public static event Action SignalFixedUpdate, SignalLateUpdate;

		public static FutilefBehaviour Instance;

		// Assigned in Awake
		public static bool IsOpenGl;
		public static float DisplayCorrection;

		// The stage
		public static Futilef.Core.Stage Stage;

		GameObject _cameraHolder;
		Camera _camera;

		void Awake() {
			Instance = this;

			IsOpenGl = SystemInfo.graphicsDeviceVersion.Contains("OpenGL");
			enabled = false;
		}

		void OnEnable() {
			SignalPreUpdate += Display.OnUpdate;
			SignalPreUpdate += TouchManager.OnUpdate; 

			Display.SignalResize += ResizeCamera;
		}

		void OnDisable() {
			SignalPreUpdate -= Display.OnUpdate;
			SignalPreUpdate -= TouchManager.OnUpdate; 

			Display.SignalResize -= ResizeCamera;
		}

		[ContextMenu("Init")]
		public void Init() {
			Init(800, 1, 1, Color.black);
		}

		public void Init(float referenceLength, float displayScaling, float resourceScaling, Color backgroundColor) {
			Stage = new Futilef.Core.Stage("main");

			enabled = true;

			Application.targetFrameRate = 30;

			Display.Init(referenceLength, displayScaling, resourceScaling);
			DisplayCorrection = IsOpenGl ? 0 : (0.5f * Display.Pixel2Display);

			_cameraHolder = new GameObject();
			_cameraHolder.transform.parent = transform;
			_cameraHolder.transform.position = new Vector3(DisplayCorrection, -DisplayCorrection, CameraOffsetZ);

			_camera = _cameraHolder.AddComponent<Camera>();
			_camera.tag = "MainCamera";
			_camera.name = "Camera";
			_camera.clearFlags = CameraClearFlags.SolidColor;
			_camera.backgroundColor = backgroundColor;
			_camera.nearClipPlane = 0.0f;
			_camera.farClipPlane = 500.0f;
			_camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
			_camera.depth = 100;
			_camera.orthographic = true;
		}
			
		public void ResizeCamera() {
			_camera.orthographicSize = Display.HalfHeight;

			Debug.LogFormat("_camera.orthographicSize={0}", _camera.orthographicSize);
		}

		void Update() {
			if (SignalPreUpdate != null) SignalPreUpdate();
			if (SignalUpdate != null) SignalUpdate();
			if (SignalAfterUpdate != null) SignalAfterUpdate();

			int depth = 0;
			Renderer.StartRender();
			Stage.Redraw(ref depth, false, false);
			Renderer.EndRender();

			if (SignalAfterDraw != null) SignalAfterDraw();
		}

		void LateUpdate() {
			if (SignalLateUpdate != null) SignalLateUpdate();
		}

		void FixedUpdate() {
			if (SignalFixedUpdate != null) SignalFixedUpdate();
		}
	}
}