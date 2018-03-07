using System.Collections.Generic;

namespace Futilef.Core {
	public abstract partial class Node {
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

		public Container container { get { return _container; } }

		// Transform
		protected float _x, _y, _z = 100;
		protected float _scalingX, _scalingY;
		protected float _rotationZ;

		// Transform matrices
		protected readonly Matrix2D _matrix = new Matrix2D(), _concatednatedMatrix = new Matrix2D();
		protected bool _isMatricesDirty;

		// Special matrices
		//		protected readonly Matrix2D _inverseConcatenatedMatrix = new Matrix2D();
		//		protected readonly Matrix2D _screenConcatenatedMatrix = new Matrix2D(), _inverseScreenConcatenatedMatrix = new Matrix2D();
		//		protected bool _needsSpecialMatrices;

		// Alpha
		protected float _alpha = 1f, _concatenatedAlpha = 1f;
		protected bool _isAlphaDirty;

		// Parent
		protected Container _container;

		// Enablers
		readonly List<Enabler> _enablers = new List<Enabler>();

		public virtual void Redraw(bool shouldForceMatricesDirty, bool shouldForceAlphaDirty) {
			if (shouldForceMatricesDirty || _isMatricesDirty) RecalculateMatrices();
			if (shouldForceAlphaDirty || _isAlphaDirty) RecalculateAlpha();
		}

		protected void RecalculateMatrices() {
			_isMatricesDirty = false;

			_matrix.FromScalingRotationTranslation(_x, _y, _scalingX, _scalingY, _rotationZ);

			if (_container != null) _concatednatedMatrix.FromMultiply(_matrix, _container._concatednatedMatrix);
			else _concatednatedMatrix.Copy(_matrix);
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
		}

		public virtual void OnRemovedFromContainer() {
			_container = null;
		}
	}
}

