﻿namespace Futilef {
	public unsafe struct PtrLst2 {
		#if FDB
		public static readonly int Type = Fdb.NewType("PtrLst2");
		public int type;
		#endif

		public int len, count;
		public void **arr;

		public static void Init(PtrLst2 *self) {
			Init(self, 4);
		}
		public static void Init(PtrLst2 *self, int len) {
			#if FDB
			Should.NotNull("self", self);
			Should.GreaterThanZero("len", len);
			self->type = Type;
			#endif
			self->len = len;
			self->count = 0;
			self->arr = (void **)Mem.Malloc(len * sizeof(void *));
		}

		public static void Push(PtrLst2 *self, void *ptr) {
			#if FDB
			Verify(self);
			#endif
			int oldCount = self->count;
			self->count = oldCount + 1;
			if (oldCount >= self->len) {  // resize
				self->arr = (void **)Mem.Realloc(self->arr, (self->len <<= 1) * sizeof(void *));
			}
			self->arr[oldCount] = ptr;
		}

		public static void RemoveAt(PtrLst2 *self, int idx) {
			#if FDB
			Verify(self);
			Should.InRange("idx", idx, 0, self->count - 1);
			#endif
			int size = sizeof(void *);
			byte* src = (byte *)(self->arr + idx);
			int count = self->count -= 1;
			Mem.Memmove(src, src + size, (count - idx) * size);
		}

		public static void ShiftPtr(PtrLst2 *self, long shift) {
			#if FDB
			Verify(self);
			#endif
			void **arr = self->arr;
			for (int i = 0, count = self->count; i < count; i += 1) {
				*(byte **)(arr + i) += shift;
			}
		}


		#if FDB
		public static void Verify(PtrLst2 *self) {
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			int len = Mem.Verify(self->arr) / sizeof(void *);
			Should.Equal("self->len", self->len, len);
			Should.InRange("self->count", self->count, 0, self->len);
		}

		public static void Test() {
			TestPush();
			TestRemoveAt();
			TestPushRemovePush();
			TestShiftPtr();
		}

		static void TestPush() {
			const int len = 1000;
			var arr = stackalloc int[len];
			var lst = stackalloc PtrLst2[1]; Init(lst);
			for (int i = 0; i < len; i += 1) {
				arr[i] = Fdb.Random(0, len);
				Push(lst, (void *)arr[i]);
			}
			void **ptr = lst->arr;
			for (int i = 0; i < len; i += 1) {
				Should.Equal("(int)ptr[i]", (int)ptr[i], arr[i]);
			}
		}

		static void TestRemoveAt() {
			const int len = 1000;
			var arr = new System.Collections.Generic.List<int>();
			var lst = stackalloc PtrLst2[1]; Init(lst);
			for (int i = 0; i < len; i += 1) {
				int v = Fdb.Random(0, len);
				arr.Add(v);
				Push(lst, (void *)v);
			}
			for (int i = 0; i < len >> 1; i += 1) {
				int idx = Fdb.Random(0, arr.Count);
				arr.RemoveAt(idx);
				RemoveAt(lst, idx);
			}
			void **ptr = lst->arr;
			for (int i = 0; i < arr.Count; i += 1) {
				Should.Equal("(int)ptr[i]", (int)ptr[i], arr[i]);
			}
		}

		static void TestPushRemovePush() {
			const int len = 1000;
			var arr = new System.Collections.Generic.List<int>();
			var lst = stackalloc PtrLst2[1]; Init(lst);
			for (int i = 0; i < len; i += 1) {
				int v = Fdb.Random(0, len);
				arr.Add(v);
				Push(lst, (void *)v);
			}
			for (int i = 0; i < len >> 1; i += 1) {
				int idx = Fdb.Random(0, arr.Count);
				arr.RemoveAt(idx);
				RemoveAt(lst, idx);
			}
			for (int i = 0; i < len; i += 1) {
				int v = Fdb.Random(0, len);
				arr.Add(v);
				Push(lst, (void *)v);
			}
			void **ptr = lst->arr;
			for (int i = 0; i < arr.Count; i += 1) {
				Should.Equal("(int)ptr[i]", (int)ptr[i], arr[i]);
			}
		}

		static void TestShiftPtr() {
			const int len = 1000;
			var arr = stackalloc int[len];
			var lst = stackalloc PtrLst2[1]; Init(lst);
			for (int i = 0; i < len; i += 1) {
				arr[i] = Fdb.Random(0, len);
				Push(lst, (void *)arr[i]);
			}
			long shift = Fdb.Random(-1000, 1000);
			ShiftPtr(lst, shift);
			void **ptr = lst->arr;
			for (int i = 0; i < len; i += 1) {
				Should.Equal("(int)ptr[i]", (long)ptr[i], arr[i] + shift);
			}
		}
		#endif
	}
}