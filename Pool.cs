#if FDB_POOL
namespace Futilef {
	public unsafe struct Pool {
		const int InitLen = 4;
		static readonly int PtrSize = sizeof(void *), TwoPtrSize = sizeof(void *) << 1;

		#if FDB
		static readonly int Type = Fdb.NewType("Pool");
		int type;
		#endif
		public int count, len;
		public byte *arr;
		public byte *first;
		public byte *free;

		int size, elmtSize;

		public static Pool *New(int size) {
			#if FDB
			Should.GreaterThan("size", size, 0);
			#endif
			return Init((Pool *)Mem.Malloc(sizeof(Pool)), size);
		}
			
		// free -> | elmt1... | next -|> elmt2... | next -|> null
		public static Pool *Init(Pool *self, int size) {
			#if FDB
			Should.NotNull("self", self);
			Should.GreaterThan("size", size, 0);
			self->type = Type;
			#endif
			int elmtSize = self->elmtSize = size;
			self->size = (size += TwoPtrSize);

			self->count = 0;
			self->len = InitLen;
			var arr = self->arr = (byte *)Mem.Malloc(InitLen * size);

			self->first = null;
			self->free = Link(arr, size, elmtSize, 0, InitLen * size);
			#if FDB
			Verify(self);
			#endif
			return self;
		}

		public static void Clear(Pool *self) {
			self->count = 0;
			self->first = null;
			int size = self->size;
			self->free = Link(self->arr, size, self->elmtSize, 0, self->len * size);
		}

		static byte *Link(byte *arr, int size, int elmtSize, int start, int end) {  // link node at start ... (end - 1)
			#if FDB
			Should.NotNull("arr", arr);
			Should.GreaterThan("size", size, 0);
			Should.Equal("elmtSize + TwoPtrSize", elmtSize + TwoPtrSize, size);
			Should.GreaterThanOrEqualTo("start", start, 0);
			Should.GreaterThan("end", end, start);
			Should.Zero("(end - start) % size", (end - start) % size);
			#endif
			for (int i = start; i < end; i += size) {
				var prePtr = (byte **)(arr + i + elmtSize);
				*prePtr = arr + i - size;
				*(prePtr + 1) = (byte *)(prePtr + 2);
			}
			*(byte **)(arr + start + elmtSize) = null;  // pre of start
			*(byte **)(arr + end - PtrSize) = null;  // next of last
			return arr + start;
		}

		static byte *LinkRev(byte *arr, int size, int elmtSize, int last, int start) {  // link node at last ... start
			for (int i = last; i >= start; i -= size) {
				var prePtr = (byte **)(arr + i + elmtSize);
				*prePtr = (byte *)(prePtr + 2);
				*(prePtr + 1) = arr + i - size;
			}
			*(byte **)(arr + last + elmtSize) = null;  // pre of last
			*(byte **)(arr + start + elmtSize + PtrSize) = null;  // next of start
			return arr + last;
		}

		public static void Decon(Pool *self) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			Verify(self);
			self->type = Fdb.NullType;
			#endif
			self->count = self->len = 0;
			Mem.Free(self->arr); self->arr = self->free = self->first = null;
		}

		public static bool Push(Pool *self) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			Verify(self);
			#endif
			int elmtSize = self->elmtSize;

			if (self->free == null) {
				var size = self->size;
				// expand
				int len = self->len <<= 1;
				var arr = self->arr = (byte *)Mem.Realloc(self->arr, self->len * size);
				// link used
				self->first = LinkRev(arr, size, elmtSize, self->count * size, 0);
				// link free
				self->free = Link(arr, size, elmtSize, (self->count += 1) * size, len * size);
				#if FDB
				Verify(self);
				#endif
				return true;
			} else {
				// remove node from free
				var prePtr = self->free + elmtSize;
				var pre = *(byte **)prePtr; 
				var next = *(byte **)(prePtr + PtrSize);
				if (pre != null) *(byte **)(pre + elmtSize + PtrSize) = next;  // pre->next = next
				if (next != null) *(byte **)(next + elmtSize) = pre;  // next->pre = pre
				// insert node to used
				if (self->first != null) *(byte **)(self->first + elmtSize) = self->free;  // first->pre = free
				*(byte **)prePtr = null;  // node->pre = null
				*(byte **)(prePtr + PtrSize) = self->first;  // node->next = first
				self->first = self->free;  // first = free
				self->free = next;  // free = next
				self->count += 1;
				#if FDB
				Verify(self);
				#endif
				return false;
			}
		}

		public static bool Push(Pool *self, byte *src) {
			#if FDB
			Should.NotNull("self", self);
			Should.NotNull("src", src);
			Should.TypeEqual("self", self->type, Type);
			Verify(self);
			#endif
			int elmtSize = self->elmtSize;

			if (self->free == null) {
				var size = self->size;
				// expand
				int len = self->len <<= 1;
				var arr = self->arr = (byte *)Mem.Realloc(self->arr, self->len * size);
				// copy
				int oldCount = self->count;
				var dst = arr + oldCount * size;
				for (int i = 0; i < elmtSize; i += 1) *dst++ = *src++;
				// link used
				self->first = LinkRev(arr, size, elmtSize, oldCount * size, 0);
				// link free
				int count = self->count += 1;
				self->free = Link(arr, size, elmtSize, count * size, len * size);
				#if FDB
				Verify(self);
				#endif
				return true;
			} else {
				// copy
				var dst = self->free;
				for (int i = 0; i < elmtSize; i += 1) *dst++ = *src++;
				// remove node from free
				var pre = *(byte **)dst; 
				var next = *(byte **)(dst + PtrSize);
				if (pre != null) *(byte **)(pre + elmtSize + PtrSize) = next;  // pre->next = next
				if (next != null) *(byte **)(next + elmtSize) = pre;  // next->pre = pre
				// insert node to used
				if (self->first != null) *(byte **)(self->first + elmtSize) = self->free;  // first->pre = free
				*(byte **)dst = null;  // node->pre = null
				*(byte **)(dst + PtrSize) = self->first;  // node->next = first
				self->first = self->free;  // first = free
				self->free = next;  // free = next
				self->count += 1;
				#if FDB
				Verify(self);
				#endif
				return false;
			}
		}

		public static int Idx(Pool *self, byte *ptr) {
			return (int)(ptr - self->arr) / self->size;
		}

		public static void RemoveAt(Pool *self, long pos) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			Should.InRange("pos", pos, 0, (self->len - 1) * self->size);
			Should.Equal("pos % self->size", pos % self->size, 0);
			var ptr = self->first;
			while (ptr != null && ptr != self->arr + pos) ptr = *(byte **)(ptr + self->elmtSize + PtrSize);
			Should.Equal("p", ptr, self->arr + pos);
			Verify(self);
			#endif
			int elmtSize = self->elmtSize;

			var node = self->arr + pos;
			var prePtr = (byte **)(node + elmtSize);
			var pre = *prePtr;
			var next = *(prePtr + 1);
			// remove from used
			if (pre != null) *(byte **)(pre + elmtSize + PtrSize) = next;  // pre->next = next
			if (next != null) *(byte **)(next + elmtSize) = pre;  // next->pre = pre
			if (self->first == node) self->first = next;
			// insert to free
			if (self->free != null) *(byte **)(self->free + elmtSize) = node;  // free->next = node
			*prePtr = null;  // node->pre = null
			*(prePtr + 1) = self->free;  // node->next = free
			self->free = node;  // free = node
			self->count -= 1;
			#if FDB
			Verify(self);
			#endif
		}

		public static void FillPtrLst(Pool *self, PtrLst *ptrLst) {
			#if FDB
			Should.NotNull("self", self);
			Should.NotNull("ptrLst", ptrLst);
			Should.GreaterThanOrEqualTo("ptrLst->len", ptrLst->len, self->count);
			Should.TypeEqual("self", self->type, Type);
			Verify(self);
			#endif
			var ptrArr = ptrLst->arr;
			int nextOff = self->elmtSize + PtrSize;
			for (var p = self->first; p != null; p = *(byte **)(p + nextOff)) *ptrArr++ = p;
		}

		#if FDB 
		public static void Test() {
			BasicPushRemoveTest();
			RandomPushRemoveTest();
		}

		static void BasicPushRemoveTest() {
			var pool = stackalloc Pool[1]; Init(pool, sizeof(int));
			uint *i = stackalloc uint[1];
			i[0] = 0xa4a4a4a4;
			Push(pool, (byte *)i);
			Push(pool, (byte *)i);
			Push(pool, (byte *)i);
			Push(pool, (byte *)i);
			Push(pool, (byte *)i);
			RemoveAt(pool, 0 * pool->size);
			RemoveAt(pool, 1 * pool->size);
			RemoveAt(pool, 4 * pool->size);
			RemoveAt(pool, 2 * pool->size);
			RemoveAt(pool, 3 * pool->size);
		}

		static void RandomPushRemoveTest() {
			var pool = stackalloc Pool[1]; Init(pool, sizeof(int));
			uint i = 0xa4a4a4a4;
			var list = new System.Collections.Generic.List<long>();
			for (int j = 0; j < 200; j += 1) {
				if (pool->count <= 0 || Fdb.Random(0, 2) == 0) {
					Push(pool, (byte *)&i);
					list.Add(pool->first - pool->arr);
					Push(pool, (byte *)&i);
					list.Add(pool->first - pool->arr);
				} else {
					var idx = Fdb.Random(0, list.Count);
					RemoveAt(pool, list[idx]);
					list.RemoveAt(idx);
				}
			}
		}

		static void Verify(Pool *self) {
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
//			Should.NotNull("self->free", self->free);
//			Fdb.Log(Fdb.Dump(self->arr, self->len * self->size, self->size));
			int elmtSize = self->elmtSize;
			int freeCount = 0, count = 0;
			for (var p = self->free; p != null; p = *(byte **)(p + elmtSize + PtrSize)) {
				Should.NotNull("(byte **)(p + elmtSize)", (byte **)(p + elmtSize));
				Should.NotNull("(byte **)(p + elmtSize + PtrSize)", (byte **)(p + elmtSize + PtrSize));
				var pre = *(byte **)(p + elmtSize);
				var next = *(byte **)(p + elmtSize + PtrSize);
				if (pre != null) {
					Should.NotNull("pre", pre);
					Should.NotNull("(byte **)(pre + elmtSize + PtrSize)", (byte **)(pre + elmtSize + PtrSize));
					Should.Equal("*(byte **)(pre + elmtSize + PtrSize)", *(byte **)(pre + elmtSize + PtrSize), p);
				}
				if (next != null) {
					Should.NotNull("next", next);
					Should.NotNull("(byte **)(next + elmtSize)", (byte **)(next + elmtSize));
					Should.Equal("*(byte **)(next + elmtSize)", *(byte **)(next + elmtSize), p);
				}
				freeCount += 1;
				Should.NotNull("(byte **)(p + elmtSize + PtrSize)", (byte **)(p + elmtSize + PtrSize));
			}
			Should.Equal("freeCount", freeCount, self->len - self->count);
//			Should.NotNull("self->first", self->first);
			for (var p = self->first; p != null; p = *(byte **)(p + elmtSize + PtrSize)) {
				var pre = *(byte **)(p + elmtSize);
				var next = *(byte **)(p + elmtSize + PtrSize);
				if (pre != null) Should.Equal("*(byte **)(pre + elmtSize + PtrSize)", *(byte **)(pre + elmtSize + PtrSize), p);
				if (next != null) Should.Equal("*(byte **)(next + elmtSize)", *(byte **)(next + elmtSize), p);
				count += 1;
			}
			Should.Equal("count", count, self->count);
		}
		#endif
	}
}
#endif