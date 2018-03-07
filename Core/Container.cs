using System.Collections.Generic;

namespace Futilef.Core {
	public class Container : Node {
		protected readonly List<Node> _children = new List<Node>();

		public override void Redraw(bool shouldForceMatricesDirty, bool shouldForceAlphaDirty) {
			shouldForceMatricesDirty = _isMatricesDirty || shouldForceMatricesDirty;
			shouldForceAlphaDirty = _isAlphaDirty || shouldForceAlphaDirty;

			base.Redraw(shouldForceMatricesDirty, shouldForceAlphaDirty);

			foreach (var child in _children) child.Redraw(shouldForceMatricesDirty, shouldForceAlphaDirty);
		}

		public int GetChildCount() {
			return _children.Count;
		}

		public Node GetChild(int index) {
			return _children[index];
		}

		public int GetChildIndex(Node child) {
			return _children.IndexOf(child);
		}
			
		public void AddChild(Node node) {
			AddChild(node, _children.Count);
		}

		public void AddChildAtBack(Node node) {
			AddChild(node, 0);
		}

		public void AddChild(Node node, int index) {
			int nodeIndex = _children.IndexOf(node);

			if (index > _children.Count) {  // If it's past the end, make it at the end
				index = _children.Count;
			}

			if (nodeIndex == index) return;  // If it's already at the right index, just leave it there

			if (nodeIndex == -1) {  // Add it if it's not a child
				node.OnAddedToContainer(this);
				_children.Insert(index, node);
			} else {  // If node is already a child, move it to the desired index
				_children.RemoveAt(nodeIndex);

				if (nodeIndex < index) _children.Insert(index - 1, node);  // Gotta subtract 1 to account for it moving in the order
				else _children.Insert(index, node);
			}
		}

		public void RemoveChild(Node node) {
			if (node.container != this) throw new System.AccessViolationException("I ain't your daddy");  // I ain't your daddy

			node.OnRemovedFromContainer();

			_children.Remove(node);
		}

		public void RemoveAllChildren() {
			int childCount = _children.Count;

			foreach (var child in _children) child.OnRemovedFromContainer();

			_children.Clear();	
		}
	}
}