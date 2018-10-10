namespace Futilef {
	public unsafe struct Lst {
		const int InitLen = 2;

		public int count, len;
		public byte *arr;

		int s;

		public static Lst *New(int s) {
			return Init((Lst *)Mem.Alloc(sizeof(Lst)), s);
		}

		public static Lst *Init(Lst *self, int s) {
			self->count = 0;
			self->len = InitLen;
			self->arr = (byte *)Mem.Alloc(InitLen * s);
			self->s = s;
			return self;
		}

		public static void Decon(Lst *self) {
			Mem.Free(self->arr);
		}

		public static void Push(Lst *self) {  // push a garbage item
			if ((self->count += 1) >= self->len) {  // resize
				self->len <<= 1;
				self->arr = (byte *)Mem.Realloc(self->arr, self->len * self->s);
			}
		}

		public static void Push(Lst *self, void *item) {
			int s = self->s;
			var dst = self->arr + self->count * s;
			var src = (byte *)item;
			for (int i = 0; i < s; i += 1) *dst++ = *src++;

			if ((self->count += 1) >= self->len) {  // resize
				self->len <<= 1;
				self->arr = (byte *)Mem.Realloc(self->arr, self->len * s);
			}
		}

		public static void *Pop(Lst *self) {
			return self->arr + (self->count -= 1) * self->s;
		}

		public static void RemoveAt(Lst *self, int idx) {
			int s = self->s;
			var dst = self->arr + idx * s;
			var src = self->arr + (idx + 1) * s;
			for (int i = 0, len = ((self->count -= 1) - idx) * s; i < len; i += 1) *dst++ = *src++;
		}

		public static void Clear(Lst *self) {
			self->count = 0;
		}

		public static void *Get(Lst *self, int idx) {
			return self->arr + idx * self->s;
		}

		public static void *Last(Lst *self) {
			return self->arr + (self->count - 1) * self->s;
		}

		public static void *End(Lst *self) {
			return self->arr + self->count * self->s;
		}

		public static string Str(Lst *self) {
			return string.Format("lst({0}, {1}, {2}, 0x{3:X})", self->s, self->count, self->len, (int)self->arr);
		}
	}
}