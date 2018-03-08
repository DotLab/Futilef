using System;

namespace Futilef.Core {
	public sealed class Stage : Container {
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
			_inverseScreenConcatenatedMatrix.FromInvert(_screenConcatenatedMatrix);
		}
	}
}

