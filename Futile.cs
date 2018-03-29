using System;
using System.Text;
using System.Collections.Generic;

using UnityEngine;

namespace Futilef {
	public sealed class Futile : MonoBehaviour {
		public const float CameraOffsetZ = -10;

		public static event Action SignalPreUpdate, SignalUpdate, SignalAfterUpdate;
		public static event Action SignalAfterDraw;
		public static event Action SignalFixedUpdate, SignalLateUpdate;

		public static Futile Instance;
		public static Futilef.Node.FStage Stage;

		public static float deltaTime { get; private set; }

		GameObject _cameraHolder;
		Camera _camera;

		void Awake() {
			Instance = this;
		}

		void OnEnable() {
			SignalPreUpdate += FScreen.OnUpdate;
			SignalPreUpdate += FTouchManager.OnUpdate;

			FScreen.SignalResize += ResizeCamera;

#if UNITY_EDITOR
//			SignalAfterUpdate += ReportStatus;
#endif
		}

		void OnDisable() {
			SignalPreUpdate -= FScreen.OnUpdate;
			SignalPreUpdate -= FTouchManager.OnUpdate; 

			FScreen.SignalResize -= ResizeCamera;

#if UNITY_EDITOR
//			SignalAfterUpdate -= ReportStatus;
#endif
		}

		public void ResizeCamera() {
			_camera.orthographicSize = FScreen.HalfHeight;

			Debug.LogFormat("_camera.orthographicSize={0}", _camera.orthographicSize);
		}

		[ContextMenu("Init")]
		void Init() {
			Init(800, 0, Color.black);

		}

		public void Init(float referenceLength, float screenScaling, Color backgroundColor) {
			Stage = new Futilef.Node.FStage("main");

			enabled = true;

			Input.simulateMouseWithTouches = false;

			_cameraHolder = new GameObject();
			_cameraHolder.transform.parent = transform;
			_cameraHolder.transform.position = new Vector3(0, 0, CameraOffsetZ);

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
			_camera.allowMSAA = false;
			_camera.allowHDR = false;

			FScreen.Init(referenceLength, screenScaling);
		}

		void Update() {
			deltaTime = Time.deltaTime;

			if (SignalPreUpdate != null) SignalPreUpdate();
			if (SignalUpdate != null) SignalUpdate();
			if (SignalAfterUpdate != null) SignalAfterUpdate();

			int depth = 0;
			Futilef.Rendering.FRenderer.StartRender();
			Stage.Redraw(ref depth, false, false);
			Futilef.Rendering.FRenderer.EndRender();

			if (SignalAfterDraw != null) SignalAfterDraw();

#if UNITY_EDITOR
//			GC.Collect();
#endif
		}

		void LateUpdate() {
			if (SignalLateUpdate != null) SignalLateUpdate();
		}

		void FixedUpdate() {
			if (SignalFixedUpdate != null) SignalFixedUpdate();
		}




















		static readonly List<WeakReference> _watchees = new List<WeakReference>();

		[TextArea]
		public string watcherStatus;

		public Texture2D texture;
		public Mesh mesh;
		public Futilef.Serialization.TpAtlas atlas;
		public Futilef.Serialization.TpFrame frame;

		public Vector2 translation, scaling;
		public float rotation;
		Shader shader;
		public Futilef.Serialization.BmFont font;

		void Start() {
			Init();
			Load();
			BuildMesh();
			LoadFont();
//			DrawText();

			shader = Shader.Find("Futilef/Basic");
			if (shader == null) throw new Exception("no shader");

			var container1 = new Futilef.Node.FContainer();
			const int count = 100;
			for (int i = 0; i < count; i++) {
				var sprite = new Futilef.Node.Display.FSprite(atlas.frameByName["こいし（不満）.png"], shader);
				sprite.x = (float)i / count * (FScreen.HalfWidth) - FScreen.HalfWidth;
				sprite.scalingX = i * 0.2f / count;
				sprite.scalingY = i * 0.2f / count;
//				sprite.alpha = 1f - (float)i / count;
				container1.AddChild(sprite);
			}
			Stage.AddChild(container1);
			var slicedSprite = new Futilef.Node.Display.FSlicedSprite(frame, shader);
			slicedSprite.scalingX = 0.4f;
			slicedSprite.scalingY = 0.4f;
			Stage.AddChild(slicedSprite);
//			var quadGroup = new Futilef.Node.Display.FQuadGroup(frame, shader);

			var label = new Futilef.Node.Display.FLabel(font, shader);
			label.horizontalAlignment = Futilef.Node.Display.FLabelHorizontalAlignment.Right;
			label.verticalAlignment = Futilef.Node.Display.FLabelVerticalAlignment.Top;
			label.x = FScreen.HalfWidth;
			label.y = FScreen.HalfHeight;
			Stage.AddChild(label);

			SignalUpdate += () => {
				label.text = (1f / Time.smoothDeltaTime).ToString("N1");
				slicedSprite.width += (float)Math.Sin(Time.time);
				slicedSprite.height += (float)Math.Cos(Time.time);
				for (int i = 0; i < count; i++) {
					container1.GetChild(i).rotationZ += ((float)i / count) * 3.14f * deltaTime;
//					Stage.GetChild(i).x += UnityEngine.Random.Range(-1f, 1f);
//					Stage.GetChild(i).y += UnityEngine.Random.Range(-1f, 1f);
				}
			};
		}

		[ContextMenu("Draw Text")]
		void DrawText() {
			var chars = "Hello World!\nIt fucking worked...".ToCharArray();
			float currentX = 0, currentY = 0;

			var vertices = new Vector3[4 * chars.Length];
			var uvs = new Vector2[vertices.Length];
			var colors = new Color32[vertices.Length];
			var triangles = new int[6 * chars.Length];

			for (int i = 0; i < chars.Length; i++) {
				if (chars[i] == '\n') {
					currentX = 0;
					currentY -= font.lineHeight;
					continue;
				}

				if (!font.HasGlyph(chars[i])) {
					Debug.LogFormat("Do not have {0}", chars[i]);
					continue;
				}

				if (i > 0) currentX += font.GetKerning(chars[i - 1], chars[i]);

				var glyph = font.GetGlyph(chars[i]);

				float posX = currentX + glyph.xOffset;
				float posY = currentY - glyph.yOffset;

				float rectLeft = posX;
				float rectRight = posX + glyph.width;
				float rectTop = posY;
				float rectBottom = posY - glyph.height;
				vertices[i * 4 + 0].Set(rectLeft, rectBottom, 0);
				vertices[i * 4 + 1].Set(rectLeft, rectTop, 0);
				vertices[i * 4 + 2].Set(rectRight, rectTop, 0);
				vertices[i * 4 + 3].Set(rectRight, rectBottom, 0);


				uvs[i * 4 + 0] = glyph.uvLeftBottom;
				uvs[i * 4 + 1] = glyph.uvLeftTop;
				uvs[i * 4 + 2] = glyph.uvRightTop;
				uvs[i * 4 + 3] = glyph.uvRightBottom;

				colors[i * 4 + 0] = new Color32(255, 255, 255, 255);
				colors[i * 4 + 1] = new Color32(255, 255, 255, 255);
				colors[i * 4 + 2] = new Color32(255, 255, 255, 255);
				colors[i * 4 + 3] = new Color32(255, 255, 255, 255);

				triangles[i * 6 + 0] = i * 4 + 0;
				triangles[i * 6 + 1] = i * 4 + 1;
				triangles[i * 6 + 2] = i * 4 + 2;
				triangles[i * 6 + 3] = i * 4 + 2;
				triangles[i * 6 + 4] = i * 4 + 3;
				triangles[i * 6 + 5] = i * 4 + 0;

				currentX += glyph.xAdvance;
			}

			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uvs;
			mesh.colors32 = colors;
		}

		[ContextMenu("Load Font")]
		void LoadFont() {
			font = FResourceManager.LoadBmFont(atlas, "Consolas.fnt");
			WatchLifetime(font);
			Debug.Log(font);
		}

		[ContextMenu("Unload Font")]
		void UnloadFont() {
			font = null;
		}

		[ContextMenu("Bind Mesh Transform")]
		void BindMeshTransform() {
			SignalUpdate += () => {
				var matrix = new FMatrix();
				matrix.FromScalingRotationTranslation(translation.x, translation.y, scaling.x, scaling.y, rotation);

				mesh.vertices = new [] { 
					matrix.Transform3D(frame.rectLeftBottom),
					matrix.Transform3D(frame.rectLeftTop),
					matrix.Transform3D(frame.rectRightTop),
					matrix.Transform3D(frame.rectRightBottom),
				};
			};
		}

		[ContextMenu("Build Mesh")]
		void BuildMesh() {
			var filter = gameObject.AddComponent<MeshFilter>();
			mesh = filter.mesh;
			var renderer = gameObject.AddComponent<MeshRenderer>();
			var material = renderer.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
			material.mainTexture = texture;

			frame = atlas.frameByName["こいし（余裕）.png"];

			var matrix = new FMatrix();
			matrix.FromScalingRotationTranslation(translation.x, translation.y, scaling.x, scaling.y, rotation);

			mesh.vertices = new [] { 
				matrix.Transform3D(frame.rectLeftBottom),
				matrix.Transform3D(frame.rectLeftTop),
				matrix.Transform3D(frame.rectRightTop),
				matrix.Transform3D(frame.rectRightBottom),
			};

			mesh.uv = new [] {
				frame.uvLeftBottom,	
				frame.uvLeftTop,
				frame.uvRightTop,
				frame.uvRightBottom,
			};

			mesh.triangles = new [] {
				0, 1, 2,
				2, 3, 0,
			};
			mesh.colors32 = new [] {
				new Color32(255, 255, 255, 255),
				new Color32(255, 255, 255, 255),
				new Color32(255, 255, 255, 255),
				new Color32(255, 255, 255, 255),
			};
		}


		[ContextMenu("Load")]
		void Load() {
			texture = FResourceManager.LoadTexture2D("packed.png");
			atlas = FResourceManager.LoadTpAtlas(texture, "packed");
			WatchLifetime(texture);
			print(atlas);
		}

		[ContextMenu("Unload")]
		void Unload() {
			Destroy(texture);
			texture = null;
		}

		public static void WatchLifetime(object obj) {
			_watchees.Add(new WeakReference(obj));
		}

		void ReportStatus() {
			var sb = new StringBuilder();

			for (int i = 0; i < _watchees.Count; i++) {
				var watchee = _watchees[i];

				if (watchee.IsAlive) {
					sb.AppendFormat("[{1}] {0} Alive\n", watchee.Target.GetHashCode(), watchee.Target.GetType().Name);
				} else {
					_watchees.RemoveAt(i);
					i -= 1;
				}
			}

			watcherStatus = sb.ToString();
		}
	}
}
