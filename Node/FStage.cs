using System;

namespace Futilef.Node {
	public sealed class FStage : FContainer {
		public readonly string name;

		public FStage(string name) {
			this.name = name;

			_stage = this;
		}

		internal override void OnAddedToStage(FStage stage) {
			throw new NotSupportedException("Cannot add Stage to Node");
		}

		internal override void OnRemovedFromStage() {
			throw new NotSupportedException("Cannot remove Stage from Node");
		}

		internal override void OnAddedToContainer(FContainer container) {
			throw new NotSupportedException("Cannot add Stage to Node");
		}

		internal override void OnRemovedFromContainer() {
			throw new NotSupportedException("Cannot add Stage from Node");
		}
	}
}

