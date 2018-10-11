namespace Futilef {
	public unsafe struct PtrLst {
		const int InitLen = 2;

		public int count, len;
		public void **arr;

		public static PtrLst *New() {
			return Init((PtrLst *)Mem.Alloc(sizeof(PtrLst)));
		}

		public static PtrLst *Init(PtrLst *self) {
			self->count = 0;
			self->len = InitLen;
			self->arr = (void **)Mem.Alloc(InitLen * sizeof(void *));
			return self;
		}

		public static void Decon(PtrLst *self) {
			self->count = 0;
			Mem.Free(self->arr);
		}

		public static void Expand(PtrLst *self, int amout) {
			if ((self->count += amout) >= self->len) {
				self->len = self->count + 1;
				self->arr = (void **)Mem.Realloc(self->arr, self->len * sizeof(void *));
			}
		}

		public static void Push(PtrLst *self) {
			if ((self->count += 1) >= self->len) {  // expand
				self->len <<= 1;
				self->arr = (void **)Mem.Realloc(self->arr, self->len * sizeof(void *));
			}
		}

		public static void Push(PtrLst *self, void *p) {
			self->arr[self->count] = p;  // assign before bound check since we expand when count == len
			if ((self->count += 1) >= self->len) {  // expand
				self->len <<= 1;
				self->arr = (void **)Mem.Realloc(self->arr, self->len * sizeof(void *));
			}
		}

		public static void *Pop(PtrLst *self) {
			return self->arr[self->count -= 1];
		}

		public static void Remove(PtrLst *self, void *p) {
			var arr = self->arr;
			int i = 0, len = self->count;
			while (i < len && arr[i] != p) i += 1;
			if (i < len) {  // arr[i] == p
				for (len = self->count -= 1; i < len; i += 1) arr[i] = arr[i + 1];
			} else {
				UnityEngine.Debug.LogError("Remove fail");
			}
		}

		public static void Clear(PtrLst *self) {
			self->count = 0;
		}

		public static void **End(PtrLst *self) {
			return self->arr + self->count;
		}

		public static void Qsort(PtrLst *self, Algo.Cmp cmp) {
			Algo.Qsort(self->arr, 0, self->count - 1, cmp);
		}

		public static string Str(PtrLst *self) {
			return string.Format("ptrlst({0}, {1}, 0x{2:X})", self->count, self->len, (int)self->arr);
		}
	}
}