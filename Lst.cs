namespace Futilef {
	public unsafe struct Lst {
		const int InitLen = 2;

		public int count, len;
		public byte *arr;

		int size;

		public static Lst *New(int size) {
			return Init((Lst *)Mem.Alloc(sizeof(Lst)), size);
		}

		public static Lst *Init(Lst *self, int size) {
			self->count = 0;
			self->len = InitLen;
			self->arr = (byte *)Mem.Alloc(InitLen * size);
			self->size = size;
			return self;
		}

		public static void Decon(Lst *self) {
			self->count = 0;
			Mem.Free(self->arr);
		}

		public static bool Push(Lst *self) {  // push a garbage item
			if ((self->count += 1) >= self->len) {  // resize
				self->len <<= 1;
				self->arr = (byte *)Mem.Realloc(self->arr, self->len * self->size);
				return true;
			}
			return false;
		}

		public static bool Push(Lst *self, byte *src) {
			int size = self->size;
			var dst = self->arr + self->count * size;
			for (int i = 0; i < size; i += 1) *dst++ = *src++;

			if ((self->count += 1) >= self->len) {  // resize
				self->len <<= 1;
				self->arr = (byte *)Mem.Realloc(self->arr, self->len * size);
				return true;
			}
			return false;
		}

		public static void *Pop(Lst *self) {
			return self->arr + (self->count -= 1) * self->size;
		}

		public static void RemoveAt(Lst *self, int idx) {
			int size = self->size;
			var dst = self->arr + idx * size;
			var src = self->arr + (idx + 1) * size;
			for (int i = 0, len = ((self->count -= 1) - idx) * size; i < len; i += 1) *dst++ = *src++;
		}

		public static void Clear(Lst *self) {
			self->count = 0;
		}

		public static void *Get(Lst *self, int idx) {
			return self->arr + idx * self->size;
		}

		public static void *Last(Lst *self) {
			return self->arr + (self->count - 1) * self->size;
		}

		public static void *End(Lst *self) {
			return self->arr + self->count * self->size;
		}

		public static void FillPtrLst(Lst *self, PtrLst *ptrLst) {
			int size = self->size;
			var ptrArr = ptrLst->arr;
			for (byte *p = self->arr, end = (byte *)End(self); p < end; p += size) *ptrArr++ = p;
		}

		public static string Str(Lst *self) {
			return string.Format("lst({0}, {1}, {2}, 0x{3:X})", self->size, self->count, self->len, (int)self->arr);
		}
	}
}