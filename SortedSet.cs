namespace Futilef {
	public unsafe struct SortedSet {
		struct Node {
			public int left, right, height, next;  // next is next free
			public void *val;
		}

		public int len, count, root, free;
		Node *nodes;

		public static void Init(SortedSet *self, int len) {
			self->len = len;
			self->count = 0;
			self->root = -1;
			self->free = 0;
			var nodes = self->nodes = (Node *)Mem.Malloc(len * sizeof(Node));
			for (int i = 0; i < len; i += 1) nodes[i].next = i + 1;
			nodes[len - 1].next = -1;
		}

		static int NewNode(SortedSet *self, void *val) {
			int free = self->free;
			if (free != -1) {  // still have free node
				var node = self->nodes + free;
				self->free = node->next;
				node->left = -1;
				node->right = -1;
				node->height = 1;
				node->next = -1;
				node->val = val;
				return free;
			} else {  // expand
				int oldLen = self->len;
				int len = self->len = oldLen << 1;
				var nodes = self->nodes = (Node *)Mem.Malloc(len * sizeof(Node));
				for (int i = oldLen + 1; i < len; i += 1) nodes[i].next = i + 1;
				nodes[len - 1].next = -1;
				var node = nodes + oldLen;
				node->left = -1;
				node->right = -1;
				node->height = 1;
				node->next = -1;
				node->val = val;
				return oldLen;
			}
		}

		static int GetBalance(Node *nodes, Node *node) {
			int left = node->left, right = node->right;
			int leftHeight = 0, rightHeight = 0;
			if (left != -1) leftHeight = nodes[left].height;
			if (right != -1) rightHeight = nodes[right].height;
			return leftHeight - rightHeight;
		}

		static void *GetMinVal(Node *nodes, Node *node) {
			int left = node->left;
			while (left != -1) node = nodes + nodes[left].left;
			return node->val;
		}

		public static void Insert(SortedSet *self, void *val, Algo.Cmp cmp) {
			self->root = InsertRecur(self, self->nodes, self->root, val, cmp);
		}
		static int InsertRecur(SortedSet *self, Node *nodes, int root, void *val, Algo.Cmp cmp) {
			if (root == -1) {  // new node and return, the base case
				return NewNode(self, val);
			}

			var rootNode = nodes + root;
			int res = cmp(val, rootNode->val);
			if (res < 0) {  // val < node->val
				rootNode->left = InsertRecur(self, nodes, rootNode->left, val, cmp);
			} else if (res > 0) {  // node->val < val
				rootNode->right = InsertRecur(self, nodes, rootNode->right, val, cmp);
			} else {  // val == node->val
				rootNode->val = val;
				return root;  // val substitude, tree not changed, base case
			}

			return Rebalance(nodes, rootNode, root);
		}

		public static void Remove(SortedSet *self, void *val, Algo.Cmp cmp) {
			self->root = RemoveRecur(self, self->nodes, self->root, val, cmp);
		}
		static int RemoveRecur(SortedSet *self, Node *nodes, int root, void *val, Algo.Cmp cmp) {
			if (root == -1) {
				return root;  // nothing to remove, base case
			}

			var rootNode = nodes + root;
			int res = cmp(val, rootNode->val);
			if (res < 0) {  // val < node->val
				rootNode->left = RemoveRecur(self, nodes, rootNode->left, val, cmp);
			} else if (res > 0) {  // node->val < val
				rootNode->right = RemoveRecur(self, nodes, rootNode->right, val, cmp);
			} else {  // val == node->val
				int left = rootNode->left, right = rootNode->right;
				if (left == -1 || right == -1) {  // no child or only 1 child
					if (left != -1) {
						var leftNode = nodes + left;
						rootNode->val = leftNode->val;
						leftNode->next = self->free;
						self->free = left;
					} else if (right != -1) {
						var rightNode = nodes + right;
						rootNode->val = rightNode->val;
						rightNode->next = self->free;
						self->free = right;
					} else {
						rootNode->next = self->free;
						self->free = root;
						return -1;
					} 
				} else {
					void *minVal = GetMinVal(nodes, nodes + right);  // inorder successor
					rootNode->val = minVal;
					rootNode->right = RemoveRecur(self, nodes, right, minVal, cmp);
				}
			}

			return Rebalance(nodes, rootNode, root);
		}

		static int Rebalance(Node *nodes, Node *rootNode, int root) {
			// calculate height
			int left = rootNode->left, right = rootNode->right;
			Node *leftNode = null; Node *rightNode = null;
			int leftHeight = 0, rightHeight = 0, maxChildHeight = 0;
			if (left != -1) {
				leftNode = nodes + left;
				leftHeight = leftNode->height;
				maxChildHeight = leftHeight;
			}
			if (right != -1) {
				rightNode = nodes + right;
				rightHeight = rightNode->height;
				if (rightHeight > maxChildHeight) maxChildHeight = rightHeight;
			}
			rootNode->height = maxChildHeight + 1;

			// calculate balance
			int balance = leftHeight - rightHeight;
			if (balance < -1) {  // leftHeight < rightHeight - 1
				int rightBalance = GetBalance(nodes, rightNode);
				if (rightBalance < 0) {  // rightLeftHeight < rightRightHeight
					return RightRightRotation(nodes, rootNode, root);
				} else {  // rightLeftHeight > rightRightHeight 
					return RightLeftRotation(nodes, rootNode, root);
				}
			} else if (balance > 1) {  // leftHeight - 1 > rightHeight
				int leftBalance = GetBalance(nodes, leftNode);
				if (leftBalance < 0) {  // leftLeftHeight < leftRightHeight
					return LeftRightRotation(nodes, rootNode, root);
				} else {  // leftLeftHeight < leftRightHeight
					return LeftLeftRotation(nodes, rootNode, root);
				}
			}

			return root;  // balanced, root not changed
		}

		/**
		 * left left
		 *       6   ->      4
		 *      / \  ->    /   \
		 *     4   7 ->   2     6
		 *    / \    ->  / \   / \
		 *   2   5   -> 1   3 5   7
		 *  / \
		 * 1   3
		 */
		static int LeftLeftRotation(Node *nodes, Node *node6, int idx6) {
			int idx4 = node6->left;
			var node4 = nodes + idx4;
			int idx5 = node4->right;
			node4->right = idx6;
			node6->left = idx5;
			return idx4;
		}

		/**
		 * left right
		 *       6   ->      4
		 *      / \  ->    /   \
		 *     2   7 ->   2     6
		 *    / \    ->  / \   / \
		 *   1   4   -> 1   3 5   7
		 *      / \
		 *     3   5
		 */
		static int LeftRightRotation(Node *nodes, Node *node6, int idx6) {
			int idx2 = node6->left;
			var node2 = nodes + idx2;
			int idx4 = node2->right;
			var node4 = nodes + idx4;
			int idx3 = node4->left, idx5 = node4->right;
			node4->left = idx2;
			node4->right = idx6;
			node2->right = idx3;
			node6->left = idx5;
			return idx4;
		}

		/**
		 * right right
		 *       2     ->      4
		 *      / \    ->    /   \
		 *     1   4   ->   2     6
		 *        / \  ->  / \   / \
		 *       3   6 -> 1   3 5   7
		 *          / \
		 *         5   7
		 */
		static int RightRightRotation(Node *nodes, Node *node2, int idx2) {
			int idx4 = node2->right;
			var node4 = nodes + idx4;
			int idx3 = node4->left;
			node4->left = idx2;
			node2->right = idx3;
			return idx4;
		}

		/**
		 * right left
		 *       2     ->      4
		 *      / \    ->    /   \
		 *     1   6   ->   2     6
		 *        / \  ->  / \   / \
		 *       4   7 -> 1   3 5   7
		 *      / \
		 *     3   5
		 */
		static int RightLeftRotation(Node *nodes, Node *node2, int idx2) {
			int idx6 = node2->right;
			var node6 = nodes + idx6;
			int idx4 = node6->left;
			var node4 = nodes + idx4;
			int idx3 = node4->left, idx5 = node4->right;
			node4->left = idx2;
			node4->right = idx6;
			node2->right = idx3;
			node2->left = idx5;
			return idx4;
		}

		#if FDB
		public static void Test() {
			
		}
		#endif
	}
}