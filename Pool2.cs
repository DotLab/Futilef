namespace Futilef {
	public unsafe struct Pool2 {
		#if FDB
		public static int Type = Fdb.NewType("Pool2");
		public int type;
		#endif

		public int len, headSize, tailSize;
		public byte *arr;

		public static void Init(Pool2 *self) {
			Init(self, 64);
		}
		public static void Init(Pool2 *self, int len) {
			#if FDB
			Should.NotNull("self", self);
			Should.GreaterThanZero("len", len);
			self->type = Type;
			#endif
			self->len = len;
			int headSize = self->headSize = 3 * sizeof(int);
			int tailSize = self->tailSize = sizeof(int);
			byte *arr = self->arr = (byte *)Mem.Malloc(len);

			// sentinel
			int *head = (int *)arr;
			head[0] = -1;  // sentinelHead.prev = -1;
			head[1] = headSize + tailSize;  // sentinelhead.next = headSize + tailSize;
			head[2] = 0;  // sentinelHead.size = 0;
			head[3] = -1;  // sentinelTail.size = -1;  // to prevent merging
//			Fdb.Dump(arr, len);
			//               sentinel              firstFree             sentinel
			int size = len - headSize - tailSize - headSize - tailSize - headSize;
			SetFreeMeta((int *)(arr + headSize + tailSize), 0, -1, size);

			// sentinel
			head = (int *)(arr + len - headSize);
			head[0] = -1;  // prev = -1 will prevent merging
			head[1] = -1;
			head[2] = 0;
			#if FDB
			Verify(self);
			#endif
		}

		/**
		 * | -1 | -1 | size | data... | -1 |
		 *                  | size    |
		 */
		static void SetUsedMeta(int *head, int size) {
			#if FDB
			Should.NotNull("head", head);
			Should.GreaterThanZero("size", size);
			#endif
			head[0] = -1; head[1] = -1; head[2] = size;
			int *tail = (int *)((byte *)(head + 3) + size);
			tail[0] = -1;
		}

		/**
		 * | prev | next | size | data... | size |
		 *                      | size    |
		 */
		static void SetFreeMeta(int *head, int prev, int next, int size) {
			#if FDB
			Should.NotNull("head", head);
			Should.GreaterThanOrEqualTo("prev", prev, -1);
			Should.GreaterThanOrEqualTo("next", next, -1);
			Should.GreaterThanZero("size", size);
			#endif
			head[0] = prev; head[1] = next; head[2] = size;
			int *tail = (int *)((byte *)(head + 3) + size);
			tail[0] = size;
		}

		/**
		 * | prev | next | size | data... | size |
		 *                      | size    |
		 */
		static void SetFreeMeta(int *head, int size) {
			#if FDB
			Should.NotNull("head", head);
			Should.GreaterThanZero("size", size);
			#endif
			head[2] = size;
			int *tail = (int *)((byte *)(head + 3) + size);
			tail[0] = size;
		}

		/**
		 * | 0 | firstFreeHead.next | size | data... | size |
		 *                                 | size    |
		 */
		static void SetFreeMetaAndInsert(byte *arr, int *head, int size) {
			#if FDB
			Should.NotNull("arr", arr);
			Should.NotNull("head", head);
			Should.GreaterThanZero("size", size);
			#endif
			int *firstFreeHead = (int *)arr;
			head[0] = 0; head[1] = firstFreeHead[1]; head[2] = size;
			firstFreeHead[1] = (int)((byte *)head - arr);  // firstFreeHead.next = head
			int *tail = (int *)((byte *)(head + 3) + size);
			tail[0] = size;
		}

		static void RemoveFromFreeList(byte *arr, int *head) {
			#if FDB
			Should.NotNull("arr", arr);
			Should.NotNull("head", head);
			#endif
			int prev = head[0], next = head[1];
			if (prev != -1) ((int *)(arr + prev))[1] = next;
			if (next != -1) ((int *)(arr + next))[0] = prev;
		}

		/**
		 * | head    | data... | tail | rightHead | data...   | rightTail |
		 *           | size    |                  | rightSize |
		 * | newHead | data...                                | tail      |
		 *           | newSize                                |
		 * assume head is free and linked
		 */
		static void MergeRight(byte *arr, int *head, int headSize, int tailSize) {
			#if FDB
			Should.NotNull("arr", arr);
			Should.NotNull("head", head);
			Should.Equal("headSize", headSize, 3 * sizeof(int));
			Should.Equal("tailSize", tailSize, sizeof(int));
			#endif
			int size = head[2];
			int *rightHead = (int *)((byte *)head + headSize + size + tailSize);
			if (rightHead[0] == -1) return;  // since only the left most sentinel has head.prev = -1, we can check this to see if the node is used

			int newSize = size;
			do {
				int rightSize = rightHead[2];
				newSize = newSize + tailSize + headSize + rightSize;
				RemoveFromFreeList(arr, rightHead);

				rightHead = (int *)((byte *)rightHead + headSize + rightSize + tailSize);
			} while (rightHead[0] != -1);

			SetFreeMeta(head, newSize);
		}

		/**
		 * | leftHead | data...  | leftTail | head | data... | tail    |
		 *            | leftSize |                 | size    |
		 * | newHead  | data...                              | newTail |
		 *            | newSize                              |
		 * assume the head is free and linked
		 */
		static int *MergeLeft(byte *arr, int *head, int headSize, int tailSize) {
			#if FDB
			Should.NotNull("arr", arr);
			Should.NotNull("head", head);
			Should.Equal("headSize", headSize, 3 * sizeof(int));
			Should.Equal("tailSize", tailSize, sizeof(int));
			#endif
			int *leftTail = head - 1;
			int leftSize = *leftTail;
			if (leftSize == -1) return head;

			RemoveFromFreeList(arr, head);
			int *leftHead;
			int newSize = head[2];
			do {
				leftHead = (int *)((byte *)leftTail - leftSize - headSize);
				RemoveFromFreeList(arr, leftHead);
				newSize = leftSize + tailSize + headSize + newSize;

				leftTail = leftHead - 1;
				leftSize = *leftTail;
			} while (leftSize != -1);

			SetFreeMeta(leftHead, newSize);
			return leftHead;
		}

		public static void *Alloc(Pool2 *self, int size) {
			#if FDB
			Verify(self);
			Should.GreaterThanZero("size", size);
			#endif
			int headSize = self->headSize, tailSize = self->tailSize;
			byte *arr = self->arr;

			int *head;
			int *freeHead;
			int freeSize;

			int curFree = 0;
			while (curFree != -1) {  // foreach free block
				head = (int *)(arr + curFree);
				int blockSize = head[2];
				if (blockSize >= size) {  // found a block that can fit
					// | head | data...                              | tail |
					//        | blockSize                            |
					// | head | data... | tail | freeHead | data...  | tail |
					//        | size    |                 | freeSize |
					freeSize = blockSize - size - tailSize - headSize;
					if (freeSize > 0) {  // split
						freeHead = (int *)((byte *)head + headSize + size + tailSize);
						SetFreeMetaAndInsert(arr, freeHead, freeSize);
						MergeRight(arr, freeHead, headSize, tailSize);

						RemoveFromFreeList(arr, head);
						SetUsedMeta(head, size);
					} else {
						RemoveFromFreeList(arr, head);
						SetUsedMeta(head, blockSize);
					}
					#if FDB
					Verify(self);
					#endif
					return head + 3;
				}
				curFree = head[1];
			}

			// expand and split
			// | data... | endHead  |
			// | data... | freeHead | data...  | freeTail | head | data... | tail | endHead |
			// | len                                                                        |
			// | oldLen             | freeSize |                 | size    |
			int oldLen = self->len, len = oldLen, minLen = oldLen + headSize + size + tailSize + headSize + tailSize;
			while (len < minLen) len <<= 1;
			self->len = len;
//			Fdb.Log("{0:X}", (long)arr);
//			Fdb.Log("len: {0}", Mem.Verify(arr));
			arr = self->arr = (byte *)Mem.Realloc(arr, len);
//			Fdb.Log("{0:X}", (long)arr);
//			Fdb.Log("len: {0}", Mem.Verify(self->arr));


			freeHead = (int *)(arr + oldLen - headSize);
			freeSize = len - oldLen - tailSize - headSize - size - tailSize - headSize;
			SetFreeMetaAndInsert(arr, freeHead, freeSize);
//			Fdb.Dump(arr, len);
			MergeLeft(arr, freeHead, headSize, tailSize);

			int *endHead = (int *)(arr + len - headSize);
			endHead[0] = -1;
			endHead[1] = -1;
			endHead[2] = 0;

			head = (int *)((byte *)freeHead + headSize + freeSize + tailSize);
			SetUsedMeta(head, size);
			#if FDB
			Verify(self);
			#endif
			return head + 3;
		}

		public static void Free(Pool2 *self, void *ptr) {
			#if FDB
			Verify(self);
			Should.InRange("ptr", ptr, self->arr, self->arr + self->len);
			#endif
			int headSize = self->headSize, tailSize = self->tailSize;
			byte *arr = self->arr;

			int *head = (int *)ptr - 3;
			SetFreeMetaAndInsert(arr, head, head[2]);
			#if FDB
			Verify(self);
			#endif
			MergeRight(arr, head, headSize, tailSize);
			#if FDB
			Verify(self);
			#endif
			MergeLeft(arr, head, headSize, tailSize);
			#if FDB
			Verify(self);
			#endif
		}

		#if FDB
		public static void Verify(Pool2 *self) {
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);

			int headSize = self->headSize, tailSize = self->tailSize;
			Should.GreaterThanZero("self->len", self->len);
			Should.Equal("self->headSize", headSize, 3 * sizeof(int));
			Should.Equal("self->tailSize", tailSize, sizeof(int));

			int len = Mem.Verify(self->arr);
			Should.Equal("self->len", self->len, len);

			Fdb.Dump(self->arr, len);

			byte *arr = self->arr;
			int *head;
			head = (int *)(arr + self->len - headSize);
			Should.Equal("endHead->prev", head[0], -1);  // head.prev = -1
			Should.Equal("endHead->next", head[1], -1);  // head.next = -1
			Should.Equal("endHead->size", head[2], 0);  // head.size = 0

			head = (int *)arr;
			Should.Equal("firstHead->prev", head[0], -1);  // head.prev = -1
			Should.Equal("firstHead->size", head[2], 0);  // head.size = 0
			Should.Equal("firstTail->size", head[3], -1);  // tail.size = -1

			int curFree = head[1], lastFree = 0;
			var dict = new System.Collections.Generic.Dictionary<int, int>();
			while (curFree != -1) {
				head = (int *)(arr + curFree);
				Should.Equal("head->prev", head[0], lastFree);
				Should.Equal("tail->size", *(int *)((byte *)head + headSize + head[2]), head[2]);
				dict.Add(curFree, head[2]);
				lastFree = curFree;
				curFree = head[1];
			}

			head = (int *)(arr + headSize + tailSize);
			int *end = (int *)(arr + self->len);
			while (head < end) {
				int pos = (int)((byte *)head - self->arr);
				int prev = head[0], next = head[1], size = head[2];
				int *tail = (int *)((byte *)head + headSize + size);

				if (tail < end) {
					int tailVal = tail[0];
					if (tailVal == -1) {  // in use
						Should.False("dict.ContainsKey(pos)", dict.ContainsKey(pos));
						Should.Equal("prev", prev, -1);
						Should.Equal("next", next, -1);
						Should.GreaterThanZero("size", size);
					} else {  // free
						Should.True("dict.ContainsKey(pos)" + pos, dict.ContainsKey(pos));
						Should.GreaterThanOrEqualTo("prev", prev, 0);
						Should.GreaterThanOrEqualTo("next", next, -1);
						Should.Equal("size", size, dict[pos]);
						dict.Remove(pos);
					}
				} else {  // head is end sentinel, no tail
					Should.Equal("head", head, (int *)(arr + len - headSize));
					Should.Equal("endHead->prev", head[0], -1);  // head.prev = -1
					Should.Equal("endHead->next", head[1], -1);  // head.next = -1
					Should.Equal("endHead->size", head[2], 0);  // head.size = 0
				}

				head = (int *)((byte *)head + headSize + size + tailSize);
			}
			Should.Zero("dict.Count", dict.Count);
		}

		public static void Test() {
			var pool = stackalloc Pool2[1]; Init(pool);
			var p = Pool2.Alloc(pool, 4);
			*(int *)p = 0x4a4a4a4a;
			Pool2.Free(pool, p);
			p = Pool2.Alloc(pool, 4);
			*(int *)p = 0x4a4a4a4a;
			p = Pool2.Alloc(pool, 4);
			*(int *)p = 0x4a4a4a4a;
			Pool2.Free(pool, p);
		}
		#endif
	}
}

