namespace Futilef {
	public unsafe struct Lst {
		const int InitLen = 4;

		#if FDB
		static readonly int Type = Fdb.NewType("Lst");
		int type;
		#endif
		public int count, len;
		public byte *arr;

		int size;

		public static Lst *New(int size) {
			#if FDB
			Should.GreaterThan("size", size, 0);
			#endif
			return Init((Lst *)Mem.Malloc(sizeof(Lst)), size);
		}

		public static Lst *Init(Lst *self, int size) {
			#if FDB
			Should.NotNull("self", self);
			Should.GreaterThan("size", size, 0);
			self->type = Type;
			#endif
			self->count = 0;
			self->len = InitLen;
			self->arr = (byte *)Mem.Malloc(InitLen * size);
			self->size = size;
			return self;
		}

		public static void Decon(Lst *self) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			self->type = Fdb.NullType;
			#endif
			self->count = self->len = 0;
			Mem.Free(self->arr); self->arr = null;
		}

		public static bool Push(Lst *self) {  // push a garbage item
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			int oldCount = self->count;
			int oldLen = self->len;
			#endif
			if ((self->count += 1) >= self->len) {  // resize
				self->len <<= 1;
				self->arr = (byte *)Mem.Realloc(self->arr, self->len * self->size);
				#if FDB
				Should.GreaterThan("self->len", self->len, oldLen);
				Should.Equal("self->count", self->count, oldCount + 1);
				#endif
				return true;
			}
			#if FDB
			Should.Equal("self->count", self->count, oldCount + 1);
			#endif
			return false;
		}

		public static bool Push(Lst *self, byte *src) {
			#if FDB
			Should.NotNull("self", self);
			Should.NotNull("src", src);
			Should.TypeEqual("self", self->type, Type);
			int oldCount = self->count;
			int oldLen = self->len;
			#endif
			int size = self->size;
			Mem.Memcpy(self->arr + self->count * size, src, size);

			if ((self->count += 1) >= self->len) {  // resize
				self->len <<= 1;
				self->arr = (byte *)Mem.Realloc(self->arr, self->len * size);
				#if FDB
				Should.GreaterThan("self->len", self->len, oldLen);
				Should.Equal("self->count", self->count, oldCount + 1);
				#endif
				return true;
			}
			#if FDB
			Should.Equal("self->count", self->count, oldCount + 1);
			#endif
			return false;
		}

		public static void *Pop(Lst *self) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			Should.GreaterThan("self->count", self->count, 0);
			#endif
			return self->arr + (self->count -= 1) * self->size;
		}

		public static void RemoveAt(Lst *self, long pos) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			Should.InRange("pos", pos, 0, (self->count - 1) * self->size);
			Should.Equal("pos % self->size", pos % self->size, 0);
			int oldCount = self->count;
			#endif
			var arr = self->arr;
			int size = self->size;
			Mem.Memcpy(arr + pos, arr + pos + size, (self->count -= 1) * size - (int)pos);
			#if FDB
			Should.Equal("self->count", self->count, oldCount - 1);
			#endif
		}

		public static void Clear(Lst *self) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			#endif
			self->count = 0;
		}

		public static void *Last(Lst *self) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			#endif
			return self->arr + (self->count - 1) * self->size;
		}

		public static void *End(Lst *self) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			#endif
			return self->arr + self->count * self->size;
		}

		public static void FillPtrLst(Lst *self, PtrLst *ptrLst) {
			#if FDB
			Should.NotNull("self", self);
			Should.NotNull("ptrLst", ptrLst);
			Should.GreaterThanOrEqualTo("ptrLst->len", ptrLst->len, self->count);
			Should.TypeEqual("self", self->type, Type);
			#endif
			int size = self->size;
			var ptrArr = ptrLst->arr;
			for (byte *p = self->arr, end = (byte *)End(self); p < end; p += size) *ptrArr++ = p;
		}

		public static string Str(Lst *self) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			#endif
			return string.Format("lst({0}, {1}, {2}, 0x{3:X})", self->size, self->count, self->len, (int)self->arr);
		}

		#if FDB
		public static void Test() {
			TestPush();
			TestPop();
			TestRemoveAt();
			TestLastEnd();
			TestFillPtrLst();
		}

		static void TestPush() {
			const int len = 100;
			var arr = stackalloc int[len];
			var lst = stackalloc Lst[1]; Init(lst, sizeof(int));
			for (int i = 0; i < len; i += 1) {
				arr[i] = Fdb.Random(0, len);
				Push(lst, (byte *)&arr[i]);
			}
			for (int i = 0; i < len; i += 1) {
				Should.Equal("*(int *)(lst->arr + i)", *(int *)(lst->arr + i * lst->size), arr[i]);
			}
		}

		static void TestPop() {
			const int len = 100;
			var arr = stackalloc int[len];
			var lst = stackalloc Lst[1]; Init(lst, sizeof(int));
			for (int i = 0; i < len; i += 1) {
				arr[i] = Fdb.Random(0, len);
				Push(lst, (byte *)&arr[i]);
			}
			for (int i = 0; i < len; i += 1) {
				Should.Equal("*(int *)Pop(lst)", *(int *)Pop(lst), arr[len - i - 1]);
			}
			Should.Equal("lst->count", lst->count, 0);
		}

		static void TestRemoveAt() {
			const int len = 100;
			var arr = new System.Collections.Generic.List<int>();
			var lst = stackalloc Lst[1]; Init(lst, sizeof(int));
			for (int i = 0; i < len; i += 1) {
				int v = Fdb.Random(0, len);
				arr.Add(v);
				Push(lst, (byte *)&v);
			}
			for (int i = 0; i < len >> 1; i += 1) {
				int idx = Fdb.Random(0, arr.Count);
				arr.RemoveAt(idx);
				RemoveAt(lst, idx * lst->size);
			}
			for (int i = 0; i < arr.Count; i += 1) {
				Should.Equal("*(int *)(lst->arr + i)", *(int *)(lst->arr + i * lst->size), arr[i]);
			}
		}

		static void TestLastEnd() {
			const int len = 100;
			var arr = stackalloc int[len];
			var lst = stackalloc Lst[1]; Init(lst, sizeof(int));
			for (int i = 0; i < len; i += 1) {
				arr[i] = Fdb.Random(0, len);
				Push(lst, (byte *)&arr[i]);
			}
			Should.Equal("*(int *)Last(lst)", *(int *)Last(lst), arr[len - 1]);
			Should.Equal("(byte *)Last(lst) - lst->arr", (byte *)Last(lst) - lst->arr, (lst->count - 1) * lst->size);
			Should.Equal("(byte *)End(lst) - lst->arr", (byte *)End(lst) - lst->arr, lst->count * lst->size);
		}

		static void TestFillPtrLst() {
			const int len = 100;
			var arr = stackalloc int[len];
			var lst = stackalloc Lst[1]; Init(lst, sizeof(int));
			for (int i = 0; i < len; i += 1) {
				arr[i] = Fdb.Random(0, len);
				Push(lst, (byte *)&arr[i]);
			}
			var ptrlst = stackalloc PtrLst[1]; PtrLst.Init(ptrlst, lst->count);
			FillPtrLst(lst, ptrlst);
			for (int i = 0; i < len; i += 1) {
				Should.Equal("(lst->arr + i)", (lst->arr + i * lst->size), ptrlst->arr[i]);
			}
		}
		#endif
	}
}