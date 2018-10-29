namespace Futilef {
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	public unsafe struct Lst2 {
		#if FDB
		public static readonly int Type = Fdb.NewType("Lst2");
		public int type;
		#endif

		public int len, count, size;
		public byte *arr;
		public long shift;

		public static void Init(Lst2 *self, int size) {
			Init(self, 4, size);
		}
		public static void Init(Lst2 *self, int len, int size) {
			#if FDB
			Should.NotNull("self", self);
			Should.GreaterThanZero("len", len);
			Should.GreaterThanZero("size", size);
			self->type = Type;
			#endif
			self->len = len;
			self->count = 0;
			self->size = size;
			self->arr = (byte *)Mem.Malloc(len * size);
			self->shift = 0;
		}

		public static void *Get(Lst2 *self, int idx) {
			#if FDB
			Verify(self);
			Should.InRange("idx", idx, 0, self->count - 1);
			#endif
			return (void *)(self->arr + idx * self->size);
		}

		public static void *Push(Lst2 *self) {
			#if FDB
			Verify(self);
			#endif
			int oldCount = self->count;
			self->count = oldCount + 1;
			if (oldCount >= self->len) {  // resize
				byte *oldArr = self->arr;
				byte *arr = self->arr = (byte *)Mem.Realloc(oldArr, (self->len <<= 1) * self->size);
				self->shift = arr - oldArr;
			}
			return (void *)(self->arr + oldCount * self->size);
		}

		public static void RemoveAt(Lst2 *self, int idx) {
			#if FDB
			Verify(self);
			Should.InRange("idx", idx, 0, self->count - 1);
			#endif
			int size = self->size;
			byte* src = self->arr + idx * size;
			int count = self->count -= 1;
			Mem.Memmove(src, src + size, (count - idx) * size);
		}

		#if FDB
		public static void Verify(Lst2 *self) {
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			int len = Mem.Verify(self->arr) / self->size;
			Should.Equal("self->len", self->len, len);
			Should.InRange("self->count", self->count, 0, self->len);
		}

		public static void Test() {
			TestPush();
			TestRemoveAt();
			TestPushRemovePush();
		}

		static void TestPush() {
			const int len = 1000;
			var arr = stackalloc int[len];
			var lst = stackalloc Lst2[1]; Init(lst, sizeof(int));
			for (int i = 0; i < len; i += 1) {
				arr[i] = Fdb.Random(0, len);
				*(int *)Push(lst) = arr[i];
			}
			int *ptr = (int *)lst->arr;
			for (int i = 0; i < len; i += 1) {
				Should.Equal("ptr[i]", ptr[i], arr[i]);
			}
		}

		static void TestRemoveAt() {
			const int len = 1000;
			var arr = new System.Collections.Generic.List<int>();
			var lst = stackalloc Lst2[1]; Init(lst, sizeof(int));
			for (int i = 0; i < len; i += 1) {
				int v = Fdb.Random(0, len);
				arr.Add(v);
				*(int *)Push(lst) = v;
			}
			for (int i = 0; i < len >> 1; i += 1) {
				int idx = Fdb.Random(0, arr.Count);
				arr.RemoveAt(idx);
				RemoveAt(lst, idx);
			}
			int *ptr = (int *)lst->arr;
			for (int i = 0; i < arr.Count; i += 1) {
				Should.Equal("ptr[i]", ptr[i], arr[i]);
			}
		}

		static void TestPushRemovePush() {
			const int len = 1000;
			var arr = new System.Collections.Generic.List<int>();
			var lst = stackalloc Lst2[1]; Init(lst, sizeof(int));
			for (int i = 0; i < len; i += 1) {
				int v = Fdb.Random(0, len);
				arr.Add(v);
				*(int *)Push(lst) = v;
			}
			for (int i = 0; i < len >> 1; i += 1) {
				int idx = Fdb.Random(0, arr.Count);
				arr.RemoveAt(idx);
				RemoveAt(lst, idx);
			}
			for (int i = 0; i < len; i += 1) {
				int v = Fdb.Random(0, len);
				arr.Add(v);
				*(int *)Push(lst) = v;
			}
			int *ptr = (int *)lst->arr;
			for (int i = 0; i < arr.Count; i += 1) {
				Should.Equal("ptr[i]", ptr[i], arr[i]);
			}
		}
		#endif
	}
}

