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
		public byte *free;
		public byte *first;

		int size, elmtSize;

		public static Pool *New(int size) {
			#if FDB
			Should.GreaterThan("size", size, 0);
			#endif
			return Init((Pool *)Mem.Alloc(sizeof(Pool)), size);
		}
			
		// free -> | elmt1... | next -|> elmt2... | next -|> null
		public static Pool *Init(Pool *self, int size) {
			#if FDB
			Should.NotNull("self", self);
			Should.GreaterThan("size", size, 0);
			self->type = Type;
			#endif
			int elmtSize = self->elmtSize = size;
			size += TwoPtrSize;
			self->size = size;
			self->count = 0;
			self->len = InitLen;
			self->arr = (byte *)Mem.Alloc(InitLen * size);

			self->first = null;
			self->free = self->arr;
			for (int i = 0, len = InitLen; i < len; i += 1) {
				var prePtr = (byte **)(self->arr + i * size + elmtSize);
				*prePtr = self->arr + (i - 1) * size;  // pre
				*(prePtr + 1) = (byte *)(prePtr + 2);  // next
			}
			*(byte **)(self->arr + elmtSize) = null;  // pre of the first
			*(byte **)(self->arr + InitLen * size - PtrSize) = null;  // next of the last
			#if FDB
			Verify(self);
			#endif
			return self;
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

		public static int Push(Pool *self, byte *src) {
			#if FDB
			Should.NotNull("self", self);
			Should.NotNull("src", src);
			Should.TypeEqual("self", self->type, Type);
			Verify(self);
			#endif
			int elmtSize = self->elmtSize;
			if (self->free == null) {
				var size = self->size;
				self->len <<= 1;
				self->arr = (byte *)Mem.Realloc(self->arr, self->len * size);
				var dst = self->arr + self->count * size;
				for (int i = 0; i < elmtSize; i += 1) *dst++ = *src++;  // copy

				self->count += 1;
				for (int i = 0, len = self->len; i < len; i += 1) {
					var prePtr = (byte **)(self->arr + i * size + elmtSize);
					*prePtr = self->arr + (i - 1) * size;  // pre
					*(prePtr + 1) = (byte *)(prePtr + 2);  // next
				}
				self->first = self->arr;
				self->free = self->arr + self->count * size;
				*(byte **)(self->arr + elmtSize) = null;  // pre of the first of used list
				*(byte **)(self->arr + self->count * size + elmtSize) = null;  // pre of the first of free list
				*(byte **)(self->arr + self->count * size - PtrSize) = null;  // next of the last of used list
				*(byte **)(self->arr + self->len * size - PtrSize) = null;  // next of the last of free list
				#if FDB
				Verify(self);
				#endif
				return (self->count - 1) * size;
			} else {
				var dst = self->free;
				for (int i = 0; i < elmtSize; i += 1) *dst++ = *src++;
				var pre = *(byte **)dst; 
				var next = *(byte **)(dst + PtrSize);
				if (pre != null) *(byte **)(pre + elmtSize + PtrSize) = next;  // pre->next = next
				if (next != null) *(byte **)(next + elmtSize) = pre;  // next->pre = pre
				if (self->first != null) *(byte **)(self->first + elmtSize) = self->free;  // first->pre = free
				*(byte **)dst = null;  // node->pre = null
				*(byte **)(dst + PtrSize) = self->first;  // node->next = first
				self->first = self->free;  // first = free
				self->free = next;  // free = next
				self->count += 1;
				#if FDB
				Verify(self);
				#endif
				return (int)(self->first - self->arr);
			}
		}

		public static void RemoveAt(Pool *self, int pos) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			Should.InRange("pos", pos, 0, self->len * self->size - 1);
			Should.Equal("pos % self->size", pos % self->size, 0);
			byte *p;
			for (p = self->first; p != null && p != self->arr + pos; p = *(byte **)(p + self->elmtSize + PtrSize));
			Should.Equal("p", p, self->arr + pos);
			Verify(self);
			#endif
			int elmtSize = self->elmtSize;
			*(self->arr + pos) = 0;
			var prePtr = (byte **)(self->arr + pos + self->elmtSize);
			var pre = *prePtr;
			var next = *(prePtr + 1);
			*prePtr = null;
			*(prePtr + 1) = self->free;
			if (self->free != null) *(byte **)(self->free + elmtSize) = self->arr + pos;
			if (self->first == self->arr + pos) self->first = next;
			if (pre != null) *(byte **)(pre + elmtSize + PtrSize) = next;  // pre->next = next
			if (next != null) *(byte **)(next + elmtSize) = pre;  // next->pre = pre
			self->free = self->arr + pos;
			self->count -= 1;
			#if FDB
			Verify(self);
			#endif
		}

		#if FDB 
		public static void Test() {
			BasicPushRemoveTest();
			RandomPushRemoveTest();
		}

		static void BasicPushRemoveTest() {
			var pool = stackalloc Pool[1]; Init(pool, sizeof(int));
			uint i = 0xa4a4a4a4;
			Push(pool, (byte *)&i);
			Push(pool, (byte *)&i);
			Push(pool, (byte *)&i);
			Push(pool, (byte *)&i);
			Push(pool, (byte *)&i);
			RemoveAt(pool, 0 * pool->size);
			RemoveAt(pool, 1 * pool->size);
			RemoveAt(pool, 2 * pool->size);
			RemoveAt(pool, 3 * pool->size);
			RemoveAt(pool, 4 * pool->size);
		}

		static void RandomPushRemoveTest() {
			var pool = stackalloc Pool[1]; Init(pool, sizeof(int));
			uint i = 0xa4a4a4a4;
			var list = new System.Collections.Generic.List<int>();
			for (int j = 0; j < 1000; j += 1) {
				list.Add(Push(pool, (byte *)&i));
				list.Add(Push(pool, (byte *)&i));
				RemoveAt(pool, list[Fdb.Random(0, list.Count - 1)]);
			}
		}

		static void Verify(Pool *self) {
			int elmtSize = self->elmtSize;
			int freeCount = 0, count = 0;
			for (var p = self->free; p != null; p = *(byte **)(p + elmtSize + PtrSize)) {
				var pre = *(byte **)(p + elmtSize);
				var next = *(byte **)(p + elmtSize + PtrSize);
				if (pre != null) Should.Equal("*(byte **)(pre + elmtSize + PtrSize)", *(byte **)(pre + elmtSize + PtrSize), p);
				if (next != null) Should.Equal("*(byte **)(next + elmtSize)", *(byte **)(next + elmtSize), p);
				freeCount += 1;
			}
			Should.Equal("freeCount", freeCount, self->len - self->count);
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

