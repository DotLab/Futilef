using System;

using UnityEngine;

namespace Futilef.Core {
	public sealed class Stage : Container {
		public readonly string name;

		public Stage(string name) {
			this.name = name;

			_stage = this;
		}

		public override void OnAddedToStage(Stage stage) {
			throw new NotSupportedException("Cannot add Stage to Node");
		}

		public override void OnRemovedFromStage() {
			throw new NotSupportedException("Cannot remove Stage from Node");
		}

		public override void OnAddedToContainer(Container container) {
			throw new NotSupportedException("Cannot add Stage to Node");
		}

		public override void OnRemovedFromContainer() {
			throw new NotSupportedException("Cannot add Stage from Node");
		}
	}
}

