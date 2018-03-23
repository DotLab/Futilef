using System;
using System.Collections.Generic;

using UnityEngine;

namespace Futilef.Node {
	public abstract class FNode : IDepthSortable {
		#region Transform

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

		#endregion

		#region Matrices

		public virtual FMatrix matrix { get { return _matrix; } }
		public virtual FMatrix concatenatedMatrix { get { return _concatenatedMatrix; } }
		public virtual FMatrix inverseConcatenatedMatrix {
			get {
				if (_shouldRecalculateInverseCancatenatedMatrix) {
					_shouldRecalculateInverseCancatenatedMatrix = false;
					_inverseConcatenatedMatrix.FromInverse(_concatenatedMatrix);
				}

				return _inverseConcatenatedMatrix;
			}
		}

		#endregion

		public FContainer container { get { return _container; } }

		public int depth { get { return _depth; } }

		// Transform
		protected float _x, _y, _z;
		protected float _scalingX = 1, _scalingY = 1;
		protected float _rotationZ;

		// Transform matrices
		protected readonly FMatrix _matrix = new FMatrix(), _concatenatedMatrix = new FMatrix();
		protected bool _isMatricesDirty = true;

		// Screen matrices (_screenConcatenatedMatrix = _concatenatedMatrix * _stage.screenConcatenatedMatrix
		protected readonly FMatrix _inverseConcatenatedMatrix = new FMatrix();
		protected bool _shouldRecalculateInverseCancatenatedMatrix;

		// Alpha
		protected float _alpha = 1f, _concatenatedAlpha = 1f;
		protected bool _isAlphaDirty = true;

		// Depth
		protected int _depth;

		// Parent
		protected FStage _stage;
		protected FContainer _container;

		// Enablers (Receives signals when Node is on stage
		readonly List<Enabler> _enablers = new List<Enabler>();

		protected FNode() {
#if UNITY_EDITOR
//			Futile.WatchLifetime(this);
#endif 
		}

		public virtual void Redraw(ref int currentDepth, bool shouldForceMatricesDirty, bool shouldForceAlphaDirty) {
			_depth = currentDepth;
			currentDepth += 1;

			if (shouldForceMatricesDirty || _isMatricesDirty) RecalculateMatrices();
			if (shouldForceAlphaDirty || _isAlphaDirty) RecalculateAlpha();
		}

		// Must be called after Redraw() since matrices are calculated during Redraw()
		public Vector2 ScreenToLocal(Vector2 position) {
			return inverseConcatenatedMatrix.Transform2D(position);
		}

		public Vector2 LocalToScreen(Vector2 position) {
			return inverseConcatenatedMatrix.Transform2D(position);
		}

		public Vector2 LocalToOther(FNode other, Vector2 position) {
			return other.ScreenToLocal(LocalToScreen(position));
		}

		#region Recalculate

		protected virtual void RecalculateMatrices() {
			_isMatricesDirty = false;

			_matrix.FromScalingRotationTranslation(_x, _y, _scalingX, _scalingY, _rotationZ);

			if (_container != null) _concatenatedMatrix.FromMultiply(_matrix, _container._concatenatedMatrix);
			else _concatenatedMatrix.FromCopy(_matrix);

			_shouldRecalculateInverseCancatenatedMatrix = true;
		}

		protected void RecalculateAlpha() {
			_isAlphaDirty = false;

			if (_container != null) _concatenatedAlpha = _alpha * _container._concatenatedAlpha;
			else _concatenatedAlpha = _alpha;
		}

		#endregion

		#region On Container

		internal virtual void OnAddedToContainer(FContainer container) {
			// Remove from the old container first if there is one
			if (_container != container) {
				if (_container != null) _container.RemoveChild(this);
				_container = container;
			}

			if (_container._stage != null) OnAddedToStage(_container._stage);
		}

		internal virtual void OnRemovedFromContainer() {
			if (_container._stage != null) OnRemovedFromStage();

			_container = null;
		}

		internal virtual void OnAddedToStage(FStage stage) {
			_stage = stage;

			foreach (var enabler in _enablers) enabler.Connect();
		}

		internal virtual void OnRemovedFromStage() {
			foreach (var enabler in _enablers) enabler.Disconnect();

			_stage = null;
		}

		#endregion

		abstract class Enabler {
			protected FNode node;

			Enabler(FNode node) {
				this.node = node;

				node._enablers.Add(this);
			}

			public abstract void Connect();
			public abstract void Disconnect();

			#region ForResize

			public sealed class ForResize : Enabler {
				Action onResize;

				public ForResize(FNode node, Action onResize) : base(node) {
					this.onResize = onResize;
				}

				public override void Connect() {
					FScreen.SignalResize += onResize;
				}

				public override void Disconnect() {
					FScreen.SignalResize -= onResize;
				}
			}

			#endregion

			#region ForTouch

			public sealed class ForSingleTouch : Enabler {
				ISingleTouchable touchable;

				public ForSingleTouch(FNode node) : base(node) {
					touchable = (ISingleTouchable)node;
				}

				public override void Connect() {
					FTouchManager.AddTouchalbe(touchable);
				}

				public override void Disconnect() {
					FTouchManager.RemoveTouchable(touchable);
				}
			}

			public sealed class ForMultiTouch : Enabler {
				IMultiTouchable touchable;

				public ForMultiTouch(FNode node) : base(node) {
					touchable = (IMultiTouchable)node;
				}

				public override void Connect() {
					FTouchManager.AddTouchalbe(touchable);
				}

				public override void Disconnect() {
					FTouchManager.RemoveTouchable(touchable);
				}
			}

			#endregion

			#region ForUpdate

			public sealed class ForPreUpdate : Enabler {
				Action onPreUpdate;

				public ForPreUpdate(FNode node, Action onPreUpdate) : base(node) {
					this.onPreUpdate = onPreUpdate;
				}

				public override void Connect() {
					Futile.SignalPreUpdate += onPreUpdate;
				}

				public override void Disconnect() {
					Futile.SignalPreUpdate -= onPreUpdate;
				}
			}

			public sealed class ForUpdate : Enabler {
				Action onUpdate;

				public ForUpdate(FNode node, Action onUpdate) : base(node) {
					this.onUpdate = onUpdate;
				}

				public override void Connect() {
					Futile.SignalUpdate += onUpdate;
				}

				public override void Disconnect() {
					Futile.SignalUpdate -= onUpdate;
				}
			}

			public sealed class ForAfterUpdate : Enabler {
				Action onAfterUpdate;

				public ForAfterUpdate(FNode node, Action onAfterUpdate) : base(node) {
					this.onAfterUpdate = onAfterUpdate;
				}

				public override void Connect() {
					Futile.SignalAfterUpdate += onAfterUpdate;
				}

				public override void Disconnect() {
					Futile.SignalAfterUpdate -= onAfterUpdate;
				}
			}

			public sealed class ForLateUpdate : Enabler {
				Action onLateUpdate;

				public ForLateUpdate(FNode node, Action onLateUpdate) : base(node) {
					this.onLateUpdate = onLateUpdate;
				}

				public override void Connect() {
					Futile.SignalLateUpdate += onLateUpdate;
				}

				public override void Disconnect() {
					Futile.SignalLateUpdate -= onLateUpdate;
				}
			}

			public sealed class ForFixedUpdate : Enabler {
				Action onFixedUpdate;

				public ForFixedUpdate(FNode node, Action onFixedUpdate) : base(node) {
					this.onFixedUpdate = onFixedUpdate;
				}

				public override void Connect() {
					Futile.SignalFixedUpdate += onFixedUpdate;
				}

				public override void Disconnect() {
					Futile.SignalFixedUpdate -= onFixedUpdate;
				}
			}

			#endregion
		}
	}
}

