using System;

using UnityEngine;

namespace Futilef.Core {
	public sealed class Stage : Container {
		public Vector3 position { get { return new Vector3(_x, _y, _z); } }
		public Vector3 scaling { get { return new Vector3(_scalingX, _scalingY, 1); } }
		public Quaternion rotation { get { return Quaternion.AngleAxis(_rotationZ, Vector3.forward); } }

		// Stage is static for its children
		public override Matrix2D matrix { get { return Matrix2D.Identity; } }
		public override Matrix2D concatenatedMatrix { get { return Matrix2D.Identity; } }

		public readonly string name;
		public int index;

		public Stage(string name) {
			this.name = name;

			_stage = this;
		}

		public override void Redraw(ref int currentDepth, bool shouldForceMatricesDirty, bool shouldForceAlphaDirty) {
			// Recalculate and clear _isMatricesDirty so that children always think I'm clean
			if (_isMatricesDirty) RecalculateMatrices();

			base.Redraw(ref currentDepth, shouldForceMatricesDirty, shouldForceAlphaDirty);
		}
			
		// Manually update screen matrices since I'm interfacing the screen
		protected override void RecalculateScreenMatrices() {
			_isScreenMatricesDirty = false;

			_screenConcatenatedMatrix.FromCopy(_matrix);
			_inverseScreenConcatenatedMatrix.FromInvert(_matrix);
		}

		public override void OnAddedToStage(Stage stage) {
			throw new NotSupportedException("Cannot add stage to Node");
		}

		public override void OnRemovedFromStage() {
			throw new NotSupportedException("Cannot remove stage from Node");
		}

		public override void OnAddedToContainer(Container container) {
			throw new NotSupportedException("Cannot add stage to Node");
		}

		public override void OnRemovedFromContainer() {
			throw new NotSupportedException("Cannot add stage from Node");
		}
	}
}

