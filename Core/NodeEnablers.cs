using System;

namespace Futilef.Core {
	public abstract partial class Node {
		protected abstract class Enabler {
			protected Node node;

			Enabler(Node node) {
				this.node = node;

				node._enablers.Add(this);
			}

			public abstract void Connect();
			public abstract void Disconnect();

			#region ForResize

			public sealed class ForResize : Enabler {
				Action onResize;

				public ForResize(Node node, Action onResize) : base(node) {
					this.onResize = onResize;
				}

				public override void Connect() {
					Display.SignalResize += onResize;
				}

				public override void Disconnect() {
					Display.SignalResize -= onResize;
				}
			}

			#endregion

			#region ForTouch

			public sealed class ForSingleTouch : Enabler {
				ISingleTouchable touchable;

				public ForSingleTouch(Node node) : base(node) {
					touchable = (ISingleTouchable)node;
				}

				public override void Connect() {
					TouchManager.AddTouchalbe(touchable);
				}

				public override void Disconnect() {
					TouchManager.RemoveTouchable(touchable);
				}
			}

			public sealed class ForMultiTouch : Enabler {
				IMultiTouchable touchable;

				public ForMultiTouch(Node node) : base(node) {
					touchable = (IMultiTouchable)node;
				}

				public override void Connect() {
					TouchManager.AddTouchalbe(touchable);
				}

				public override void Disconnect() {
					TouchManager.RemoveTouchable(touchable);
				}
			}

			#endregion

			#region ForUpdate

			public sealed class ForPreUpdate : Enabler {
				Action onPreUpdate;

				public ForPreUpdate(Node node, Action onPreUpdate) : base(node) {
					this.onPreUpdate = onPreUpdate;
				}

				public override void Connect() {
					FutilefBehaviour.SignalPreUpdate += onPreUpdate;
				}

				public override void Disconnect() {
					FutilefBehaviour.SignalPreUpdate -= onPreUpdate;
				}
			}

			public sealed class ForUpdate : Enabler {
				Action onUpdate;

				public ForUpdate(Node node, Action onUpdate) : base(node) {
					this.onUpdate = onUpdate;
				}

				public override void Connect() {
					FutilefBehaviour.SignalUpdate += onUpdate;
				}

				public override void Disconnect() {
					FutilefBehaviour.SignalUpdate -= onUpdate;
				}
			}

			public sealed class ForAfterUpdate : Enabler {
				Action onAfterUpdate;

				public ForAfterUpdate(Node node, Action onAfterUpdate) : base(node) {
					this.onAfterUpdate = onAfterUpdate;
				}

				public override void Connect() {
					FutilefBehaviour.SignalAfterUpdate += onAfterUpdate;
				}

				public override void Disconnect() {
					FutilefBehaviour.SignalAfterUpdate -= onAfterUpdate;
				}
			}

			public sealed class ForLateUpdate : Enabler {
				Action onLateUpdate;

				public ForLateUpdate(Node node, Action onLateUpdate) : base(node) {
					this.onLateUpdate = onLateUpdate;
				}

				public override void Connect() {
					FutilefBehaviour.SignalLateUpdate += onLateUpdate;
				}

				public override void Disconnect() {
					FutilefBehaviour.SignalLateUpdate -= onLateUpdate;
				}
			}

			public sealed class ForFixedUpdate : Enabler {
				Action onFixedUpdate;

				public ForFixedUpdate(Node node, Action onFixedUpdate) : base(node) {
					this.onFixedUpdate = onFixedUpdate;
				}

				public override void Connect() {
					FutilefBehaviour.SignalFixedUpdate += onFixedUpdate;
				}

				public override void Disconnect() {
					FutilefBehaviour.SignalFixedUpdate -= onFixedUpdate;
				}
			}

			#endregion
		}
	}
}

