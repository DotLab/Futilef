namespace Futilef {
	public unsafe struct PtrLst {
		const int InitLen = 4;

		#if FDB
		static readonly int Type = Fdb.NewType("PtrLst");
		int type;
		#endif
		public int count, len;
		public void **arr;

		public static PtrLst *New() {
			return Init((PtrLst *)Mem.Malloc(sizeof(PtrLst)));
		}

		public static PtrLst *Init(PtrLst *self) {
			#if FDB
			Should.NotNull("self", self);
			self->type = Type;
			#endif
			self->count = 0;
			self->len = InitLen;
			self->arr = (void **)Mem.Malloc(InitLen * sizeof(void *));
			return self;
		}
		public static PtrLst *Init(PtrLst *self, int len) {
			#if FDB
			Should.NotNull("self", self);
			Should.GreaterThan("len", len, 0);
			self->type = Type;
			#endif
			self->count = 0;
			self->len = len;
			self->arr = (void **)Mem.Malloc(len * sizeof(void *));
			return self;
		}

		public static void Decon(PtrLst *self) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			self->type = Fdb.NullType;
			#endif
			self->count = self->len = 0;
			Mem.Free(self->arr); self->arr = null;
		}

		public static void Push(PtrLst *self) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			int oldCount = self->count;
			int oldLen = self->len;
			#endif
			if ((self->count += 1) >= self->len) {  // expand
				self->len <<= 1;
				self->arr = (void **)Mem.Realloc(self->arr, self->len * sizeof(void *));
				#if FDB
				Should.GreaterThan("self->len", self->len, oldLen);
				#endif
			}
			#if FDB
			Should.Equal("self->count", self->count, oldCount + 1);
			#endif
		}

		public static void Push(PtrLst *self, void *p) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			int oldCount = self->count;
			int oldLen = self->len;
			#endif
			self->arr[self->count] = p;  // assign before bound check since we expand when count == len
			if ((self->count += 1) >= self->len) {  // expand
				self->len <<= 1;
				self->arr = (void **)Mem.Realloc(self->arr, self->len * sizeof(void *));
				#if FDB
				Should.GreaterThan("self->len", self->len, oldLen);
				#endif
			}
			#if FDB
			Should.Equal("self->count", self->count, oldCount + 1);
			#endif
		}

		public static void *Pop(PtrLst *self) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			Should.GreaterThan("size->count", self->count, 0);
			#endif
			return self->arr[self->count -= 1];
		}

		public static void Remove(PtrLst *self, void *p) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			int oldCount = self->count;
			#endif
			var arr = self->arr;
			int i = 0, len = self->count;
			while (i < len && arr[i] != p) i += 1;
			if (i < len) {  // arr[i] == p
				for (len = self->count -= 1; i < len; i += 1) arr[i] = arr[i + 1];
			}
			#if FDB
			else Fdb.Error("{0} does not exist in PtrLst {1}", (ulong)p, (ulong) self);
			Should.Equal("self->count", self->count, oldCount - 1);
			#endif
		}

		public static void Clear(PtrLst *self) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			#endif
			self->count = 0;
		}

		public static void **End(PtrLst *self) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			#endif
			return self->arr + self->count;
		}

		public static void Qsort(PtrLst *self, Algo.Cmp cmp) {
			#if FDB
			Should.NotNull("self", self);
			Should.NotNull("cmp", cmp);
			Should.TypeEqual("self", self->type, Type);
			#endif
			Algo.QuickSort(self->arr, self->count, cmp);
		}

		public static string Str(PtrLst *self) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			#endif
			return string.Format("ptrlst({0}, {1}, 0x{2:X})", self->count, self->len, (int)self->arr);
		}

		#if FDB
		public static void Test() {
			TestPush();
			TestPop();
			TestRemove();
			TestEnd();
			TestQsort();
		}

		static void TestPush() {
			const int len = 100;
			var arr = stackalloc int[len];
			var lst = stackalloc PtrLst[1]; Init(lst, sizeof(int));
			for (int i = 0; i < len; i += 1) {
				arr[i] = Fdb.Random(-len, len);
				Push(lst, (void *)arr[i]);
			}
			for (int i = 0; i < len; i += 1) {
				Should.Equal("(int)lst->arr[i]", (int)lst->arr[i], arr[i]);
			}
		}

		static void TestPop() {
			const int len = 100;
			var arr = stackalloc int[len];
			var lst = stackalloc PtrLst[1]; Init(lst, sizeof(int));
			for (int i = 0; i < len; i += 1) {
				arr[i] = Fdb.Random(-len, len);
				Push(lst, (void *)arr[i]);
			}
			for (int i = 0; i < len; i += 1) {
				Should.Equal("(int)lst->arr[i]", (int)Pop(lst), arr[len - i - 1]);
			}
			Should.Equal("lst->count", lst->count, 0);
		}

		static void TestRemove() {
			const int len = 100;
			var arr = new System.Collections.Generic.List<int>();
			var lst = stackalloc PtrLst[1]; Init(lst, sizeof(int));
			for (int i = 0; i < len; i += 1) {
				int v = Fdb.Random(0, len);
				arr.Add(v);
				Push(lst, (void *)v);
			}
			for (int i = 0; i < len >> 1; i += 1) {
				int v = arr[Fdb.Random(0, arr.Count)];
				arr.Remove(v);
				Remove(lst, (void *)v);
			}
			for (int i = 0; i < arr.Count; i += 1) {
				Should.Equal("(int)lst->arr[i]", (int)lst->arr[i], arr[i]);
			}
		}

		static void TestEnd() {
			const int len = 100;
			var lst = stackalloc PtrLst[1]; Init(lst);
			for (int i = 0; i < len; i += 1) {
				var v = Fdb.Random(-len, len);
				Push(lst, (void *)v);
			}
			Should.Equal("End(lst) - lst->arr", End(lst) - lst->arr, lst->count);
		}

		static void TestQsort() {
			const int len = 100;
			var lst = stackalloc PtrLst[1]; Init(lst);
			for (int i = 0; i < len; i += 1) {
				var v = Fdb.Random(-len, len);
				Push(lst, (void *)v);
			}
			Qsort(lst, (a, b) => (int)a - (int)b);
			for (int i = 1; i < len; i += 1) {
				Should.LessThanOrEqualTo("(int)lst->arr[i - 1]", (int)lst->arr[i - 1], (int)lst->arr[i]);
			}
		}
		#endif
	}
}