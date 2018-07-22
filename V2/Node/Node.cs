namespace Futilef.V2.Node {
	public class Node {
		public float x { get { return _x; } set { _isMatDirty = _x != value; _x = value; } }
		public float y { get { return _y; } set { _isMatDirty = _y != value; _y = value; } }
		public float z { get { return _z; } set { _isMatDirty = _z != value; _z = value; } }

		public float rotX { get { return _rotX; } set { _isRotDirty = _rotX != value; _rotX = value; _isMatDirty = _isRotDirty; } }
		public float rotY { get { return _rotY; } set { _isRotDirty = _rotY != value; _rotY = value; _isMatDirty = _isRotDirty; } }
		public float rotZ { get { return _rotZ; } set { _isRotDirty = _rotZ != value; _rotZ = value; _isMatDirty = _isRotDirty; } }
		
		public float sclX { get { return _sclX; } set { _isMatDirty = _sclX != value; _sclX = value; } }
		public float sclY { get { return _sclY; } set { _isMatDirty = _sclY != value; _sclY = value; } }
		public float sclZ { get { return _sclZ; } set { _isMatDirty = _sclZ != value; _sclZ = value; } }

		public float[] mat { get { return _mat; } }
		public float[] concatMat { get { return _concatMat; } }
		public float[] inverseConcatMat { get {
				if (_shouldUpdateInverseConcatMat) {
					_shouldUpdateInverseConcatMat = false;
					Mat4.Invert(_inverseConcatMat, concatMat);
				}
				return _inverseConcatMat;
			}
		}

		protected float _x, _y, _z;
		protected float _rotX, _rotY, _rotZ;
		protected float _sclX = 1, _sclY = 1, _sclZ = 1;

		protected readonly float[] _quat = Quat.Create();
		protected readonly float[] _mat = Mat4.Create(), _concatMat = Mat4.Create(), _inverseConcatMat = Mat4.Create();
		protected bool _isRotDirty, _isMatDirty, _shouldUpdateInverseConcatMat;

		protected Container _parent;

		public virtual void SetParent(Container parent) {
			_parent = parent;
			_isMatDirty = true;
		}

		public virtual void ClearParent() {
			_parent = null;
			_isMatDirty = true;
		}

		public virtual void Redraw() {
			if (_isRotDirty) {
				_isRotDirty = false;
				Quat.FromEuler(_quat, _rotX, _rotY, _rotZ);
			}

			if (_isMatDirty) {
				_isMatDirty = false;
				Mat4.FromRotationTranslationScale(_mat, _quat, _x, _y, _z, _sclX, _sclY, _sclZ);
				_shouldUpdateInverseConcatMat = true;

				if (_parent != null) {
					Mat4.Mul(_concatMat, _mat, _parent._concatMat);
				} else {
					Mat4.Copy(_concatMat, _mat);
				}
			}
		}

		public float[] ScreenToLocal(float[] pos) {
			if (_shouldUpdateInverseConcatMat) {
				_shouldUpdateInverseConcatMat = false;
				Mat4.Invert(_inverseConcatMat, _concatMat);
			}
			return Vec3.TransformMat4(pos, pos, _inverseConcatMat);
		}

		public float[] LocalToScreen(float[] pos) {
			return Vec3.TransformMat4(pos, pos, _concatMat);
		}
	}
}

