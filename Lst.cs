namespace Futilef {
	public unsafe struct Lst {
		const int InitLen = 2;

		public int count, len;
		public byte *ptr;

		int s;

		public static Lst *New(int s) {
			return Init((Lst *)Mem.Alloc(sizeof(Lst)), s);
		}

		public static Lst *Init(Lst *self, int s) {
			self->count = 0;
			self->len = InitLen;
			self->ptr = (byte *)Mem.Alloc(InitLen * s);
			self->s = s;
			return self;
		}

		public static void Decon(Lst *self) {
			Mem.Free(self->ptr);
		}

		public static void Push(Lst *self, void *item) {
			int s = self->s;
			int idx = self->count * s;
			if ((self->count += 1) >= self->len) {  // resize
				self->len <<= 1;
				self->ptr = (byte *)Mem.Realloc(self->ptr, self->len * s);
			}
			var dst = self->ptr + idx;
			var src = (byte *)item;
			for (int i = 0; i < s; i += 1) *dst++ = *src++;
		}

		public static void *Pop(Lst *self) {
			if (self->count <= 0) return null;
			return self->ptr + (self->count -= 1) * self->s;
		}

		public static void *Get(Lst *self, int idx) {
			return self->ptr + idx * self->s;
		}

		public static string Str(Lst *self) {
			return string.Format("lst({0}, {1}, {2}, 0x{3:X})", self->s, self->count, self->len, (int)self->ptr);
		}
	}
}