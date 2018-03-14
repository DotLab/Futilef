﻿using System;

using UnityEngine;

namespace Futilef {
	public enum PrimitiveType {
		Triangle,
		Quad,
	}

	public abstract class RenderLayer {
		public const int MinExpansionAmount = 16;
		public const int MaxUnusedAmount = 64;

		static int _count;

		public readonly int index;
		public readonly Atlas atlas;
		public readonly Shader shader;
		public abstract PrimitiveType type { get; }

		protected readonly GameObject _gameObject;
		protected readonly Transform _transform;
		protected readonly Material _material;
		protected readonly MeshFilter _meshFilter;
		protected readonly MeshRenderer _meshRenderer;
		protected readonly Mesh _mesh;

		protected Vector3[] _vertices = new Vector3[0];
		protected Color32[] _colors = new Color32[0];
		protected Vector2[] _uvs = new Vector2[0];
		protected int[] _triangles = new int[0];
		protected bool _isVerticesDirty, _isUvsDirty, _isColorsDirty;
		protected bool _didPrimitiveCountChange;

		protected int _currentPrimitiveIndex, _maxPrimitiveCount, _lowestUnusedPrimitiveIndex;
		protected int _renderQueue;

		protected string activeName { get { return string.Format("RL{0} {1} [{2}/{3}] ({4} {5} {6})", index, _renderQueue, _currentPrimitiveIndex, _maxPrimitiveCount, atlas.name, shader.name, type); } }
		protected string inactiveName { get { return string.Format("RL{0} {1} [{2}/{3}] ({4} {5} {6})", index, "X", _currentPrimitiveIndex, _maxPrimitiveCount, atlas.name, shader.name, type); } }

		protected RenderLayer(Atlas atlas, Shader shader) {
			index = _count++;
			this.atlas = atlas;
			this.shader = shader;

			_gameObject = new GameObject(inactiveName);
			_gameObject.SetActive(false);
			_transform = _gameObject.transform;
			_transform.parent = FutilefBehaviour.Instance.transform;

			_meshFilter = _gameObject.AddComponent<MeshFilter>();
			_meshRenderer = _gameObject.AddComponent<MeshRenderer>();
			_meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			_meshRenderer.receiveShadows = false;

			_material = new Material(shader.shader);
			_material.mainTexture = atlas.texture;
			shader.SetProperties(_material);

			_meshRenderer.sharedMaterial = _material;

			_mesh = _meshFilter.mesh;
			_mesh.MarkDynamic();
		}

		#region Set Dirty

		public void SetVerticesDirty() {
			_isVerticesDirty = true;
		}

		public void SetColorsDirty() {
			_isColorsDirty = true;

		}
		public void SetUvsDirty() {
			_isUvsDirty = true;
		}

		#endregion

		public abstract int PrimitiveIndexToVertexIndex(int primitiveIndex);
		public abstract int PrimitiveIndexToTriangleIndex(int primitiveIndex);
		public abstract void WindPrimitive(int start, int end);

		public void Open(int renderQueue) {
			_currentPrimitiveIndex = 0;

			if (renderQueue != _renderQueue) {
				_renderQueue = renderQueue;
				_material.renderQueue = _renderQueue;
			}
		}

		public int GetQuota(int primativeCount) {
			int lastPrimitiveIndex = _currentPrimitiveIndex;
			_currentPrimitiveIndex += primativeCount;
			if (_currentPrimitiveIndex > _vertices.Length) {
				var newSize = _currentPrimitiveIndex + 1;
				Expand(Math.Max(newSize, lastPrimitiveIndex + MinExpansionAmount));
			}

			return lastPrimitiveIndex;
		}

		public void Close() {
			if (_currentPrimitiveIndex < _maxPrimitiveCount - MaxUnusedAmount) Shrink(_currentPrimitiveIndex + 1);

			FillUnused(_currentPrimitiveIndex, _lowestUnusedPrimitiveIndex);
			_lowestUnusedPrimitiveIndex = _currentPrimitiveIndex;

			// Old Update()
			if (_didPrimitiveCountChange) {
				_didPrimitiveCountChange = false;

				_mesh.vertices = _vertices;
				_mesh.colors32 = _colors;
				_mesh.uv = _uvs;
				_mesh.triangles = _triangles;

				_mesh.bounds = new Bounds(Vector3.zero, new Vector3(9999999999, 9999999999, 9999999999));
			} else {
				if (_isVerticesDirty) _mesh.vertices = _vertices;
				if (_isColorsDirty) _mesh.colors32 = _colors;
				if (_isUvsDirty) _mesh.uv = _uvs;
			}

			_isVerticesDirty = _isColorsDirty = _isUvsDirty = false;

			#if UNITY_EDITOR
			_gameObject.name = activeName;
			#endif
		}

		protected void Expand(int newSize) {
			int firstNewPrimitiveIndex = _maxPrimitiveCount;
			_maxPrimitiveCount = newSize;

			int newVertexCount = PrimitiveIndexToVertexIndex(_maxPrimitiveCount);
			Array.Resize(ref _vertices, newVertexCount);
			Array.Resize(ref _uvs, newVertexCount);
			Array.Resize(ref _colors, newVertexCount);

			int newTriangleCount = PrimitiveIndexToTriangleIndex(_maxPrimitiveCount);
			Array.Resize(ref _triangles, newTriangleCount);

			WindPrimitive(firstNewPrimitiveIndex, _maxPrimitiveCount);
			FillUnused(firstNewPrimitiveIndex, _maxPrimitiveCount);

			_didPrimitiveCountChange = true;
		}

		protected void Shrink(int newSize) {
			_maxPrimitiveCount = newSize;

			int newVertexCount = PrimitiveIndexToVertexIndex(_maxPrimitiveCount);
			Array.Resize(ref _vertices, newVertexCount);
			Array.Resize(ref _uvs, newVertexCount);
			Array.Resize(ref _colors, newVertexCount);

			int newTriangleCount = PrimitiveIndexToTriangleIndex(_maxPrimitiveCount);
			Array.Resize(ref _triangles, newTriangleCount);

			_didPrimitiveCountChange = true;
		}

		protected void FillUnused(int start, int end) {
			int endVertexIndex = PrimitiveIndexToVertexIndex(end + 1);
			for (int i = PrimitiveIndexToVertexIndex(start); i < endVertexIndex; i++) _vertices[i].Set(50, 0, 1000000);
		}

		public void Activate() {
			_gameObject.SetActive(true);
		}

		public void Deactivate() {
			_gameObject.SetActive(false);
			#if UNITY_EDITOR
			_gameObject.name = inactiveName;
			#endif
		}

		#region Primitives

		public sealed class Triangle : RenderLayer {
			public override PrimitiveType type { get { return PrimitiveType.Triangle; } }

			public Triangle(Atlas atlas, Shader shader) : base(atlas, shader) {
			}

			public override int PrimitiveIndexToVertexIndex(int primitiveIndex) {
				return primitiveIndex * 3;
			}

			public override int PrimitiveIndexToTriangleIndex(int primitiveIndex) {
				return primitiveIndex * 3;
			}

			public override void WindPrimitive(int start, int end) {
				end *= 3;
				for (int i = start * 3; i < end; i++) _triangles[i] = i;
			}
		}

		public sealed class Quad : RenderLayer {
			public override PrimitiveType type { get { return PrimitiveType.Quad; } }

			public Quad(Atlas atlas, Shader shader) : base(atlas, shader) {
			}

			public override int PrimitiveIndexToVertexIndex(int primitiveIndex) {
				return primitiveIndex * 4;
			}

			public override int PrimitiveIndexToTriangleIndex(int primitiveIndex) {
				return primitiveIndex * 6;
			}

			public override void WindPrimitive(int start, int end) {
				end *= 4;
				for (int i = start * 4, j = start * 6; i < end; i += 4, j += 6) {
					_triangles[j] = i;
					_triangles[j + 1] = i + 1;
					_triangles[j + 2] = i + 2;

					_triangles[j + 3] = i;
					_triangles[j + 4] = i + 2;
					_triangles[j + 5] = i + 3;
				}
			}
		}

		#endregion
	}
}
