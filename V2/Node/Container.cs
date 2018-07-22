using System.Collections.Generic;

namespace Futilef.V2.Node {
	public class Container : Node {
		protected readonly List<Node> _children = new List<Node>();

		public override void Redraw() {
			base.Redraw();

			int count = _children.Count;
			for (int i = 0; i < count; i++) {
				_children[i].Redraw();
			}
		}

		public virtual void AddChild(Node child) {
			_children.Add(child);
			child.SetParent(this);
		}

		public virtual void RemoveChild(Node child) {
			_children.Remove(child);
			child.ClearParent();
		}
	}
}

