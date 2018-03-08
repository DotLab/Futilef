using System.Collections.Generic;

using UnityEngine;

namespace Futilef.Core {
	public abstract partial class Node : IDepthSortable {
		#region Transform

		public Vector3 position { get { return new Vector3(_x, _y, _z); } }
		public float x {
			get { return _x; }
			set {
				_x = value;
				_isMatricesDirty = true;
			}
		}
		public float y {
			get { return _y; }
			set {
				_y = value;
				_isMatricesDirty = true;
			}
		}
		public float z {
			get { return _y; }
			set { _z = value; }
		}

		public Vector3 scaling { get { return new Vector3(_scalingX, _scalingY, 1); } }
		public float scalingX {
			get { return _scalingX; }
			set {
				_scalingX = value;
				_isMatricesDirty = true;
			}
		}
		public float scalingY {
			get { return _scalingY; }
			set {
				_scalingY = value;
				_isMatricesDirty = true;
			}
		}

		public Quaternion rotation { get { return Quaternion.AngleAxis(_rotationZ, Vector3.forward); } }
		public float rotationZ {
			get { return _rotationZ; }
			set {
				_rotationZ = value;
				_isMatricesDirty = true;
			}
		}

		public float alpha {
			get { return alpha; }
			set {
				_alpha = value;
				_isAlphaDirty = true;
			}
		}

		#endregion

		#region Matrices

		public virtual Matrix2D matrix { get { return _matrix; } }
		public virtual Matrix2D concatenatedMatrix { get { return _concatenatedMatrix; } }

		public virtual Matrix2D screenConcatenatedMatrix {
			get {
				if (!_needsSpecialMatrices) _needsSpecialMatrices = true;
				if (_isScreenMatricesDirty) RecalculateScreenMatrices();
				return _screenConcatenatedMatrix;
			}
		}
		public virtual Matrix2D inverseScreenConcatenatedMatrix {
			get {
				if (!_needsSpecialMatrices) _needsSpecialMatrices = true;
				if (_isScreenMatricesDirty) RecalculateScreenMatrices();
				return _inverseScreenConcatenatedMatrix;
			}
		}

		#endregion

		public Container container { get { return _container; } }

		public int depth { get { return _depth; } }

		// Transform
		protected float _x, _y, _z;
		protected float _scalingX, _scalingY;
		protected float _rotationZ;

		// Transform matrices
		protected readonly Matrix2D _matrix = new Matrix2D(), _concatenatedMatrix = new Matrix2D();
		protected bool _isMatricesDirty;

		// Special matrices
		protected readonly Matrix2D _screenConcatenatedMatrix = new Matrix2D(), _inverseScreenConcatenatedMatrix = new Matrix2D();
		protected bool _needsSpecialMatrices, _isScreenMatricesDirty;

		// Alpha
		protected float _alpha = 1f, _concatenatedAlpha = 1f;
		protected bool _isAlphaDirty;

		// Depth
		protected int _depth;

		// Parent
		protected Container _container;
		protected Stage _stage;

		// Enablers
		readonly List<Enabler> _enablers = new List<Enabler>();

		protected Node() {
			#if UNITY_EDITOR
			RXProfiler.TrackLifeCycle(this);
			#endif 
		}

		public virtual void Redraw(ref int currentDepth, bool shouldForceMatricesDirty, bool shouldForceAlphaDirty) {
			_depth = currentDepth;
			currentDepth += 1;

			if (shouldForceMatricesDirty || _isMatricesDirty) RecalculateMatrices();
			if (shouldForceAlphaDirty || _isAlphaDirty) RecalculateAlpha();
		}

		protected void RecalculateMatrices() {
			_isMatricesDirty = false;

			_matrix.FromScalingRotationTranslation(_x, _y, _scalingX, _scalingY, _rotationZ);

			if (_container != null) _concatenatedMatrix.FromMultiply(_matrix, _container._concatenatedMatrix);
			else _concatenatedMatrix.FromCopy(_matrix);

			_isScreenMatricesDirty = true;
		}

		protected virtual void RecalculateScreenMatrices() {
			_isScreenMatricesDirty = false;

			if (_stage != null) _screenConcatenatedMatrix.FromMultiply(_concatenatedMatrix, _stage.screenConcatenatedMatrix);
			else _screenConcatenatedMatrix.FromCopy(_concatenatedMatrix);

			_inverseScreenConcatenatedMatrix.FromInvert(_screenConcatenatedMatrix);
		}

		protected void RecalculateAlpha() {
			_isAlphaDirty = false;

			if (_container != null) _concatenatedAlpha = _alpha * _container._concatenatedAlpha;
			else _concatenatedAlpha = _alpha;
		}

		public virtual void OnAddedToContainer(Container container) {
			// Remove from the old container first if there is one
			if (_container != container) {
				if (_container != null) _container.RemoveChild(this);
				_container = container;
			}

			if (_container._stage != null) OnAddedToStage(_container._stage);
		}

		public virtual void OnRemovedFromContainer() {
			if (_container._stage != null) OnRemovedFromStage();

			_container = null;
		}

		public virtual void OnAddedToStage(Stage stage) {
			_stage = stage;

			foreach (var enabler in _enablers) enabler.Connect();
		}

		public virtual void OnRemovedFromStage() {
			foreach (var enabler in _enablers) enabler.Disconnect();

			_stage = null;
		}

		// Must be called after Redraw() since matrices are calculated during Redraw()
		public Vector2 ScreenToLocal(Vector2 position) {
			return inverseScreenConcatenatedMatrix.Transform2D(position);
		}

		public Vector2 ScreenToDisplay(Vector2 position) {
			return screenConcatenatedMatrix.Transform2D(position);
		}

		public Vector2 LocalToOther(Node other, Vector2 position) {
			return other.ScreenToLocal(ScreenToDisplay(position));
		}
	}
}

