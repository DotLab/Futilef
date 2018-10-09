namespace Futilef {
	public unsafe struct Lstp {
		const int InitLen = 2;

		public int count, len;
		public void **ptr;

		public static Lstp *New() {
			return Init((Lstp *)Mem.Alloc(sizeof(Lstp)));
		}

		public static Lstp *Init(Lstp *self) {
			self->count = 0;
			self->len = InitLen;
			self->ptr = (void **)Mem.Alloc(InitLen * sizeof(void *));
			return self;
		}

		public static void Decon(Lstp *self) {
			Mem.Free(self->ptr);
		}

		public static void Push(Lstp *self, void *p) {
			self->ptr[self->count] = p;  // assign before bound check since we expand when count == len
			if ((self->count += 1) >= self->len) {  // expand
				self->len <<= 1;
				self->ptr = (void **)Mem.Realloc(self->ptr, self->len * sizeof(void *));
			}
		}

		public static void *Pop(Lstp *self) {
			return self->count <= 0 ? null : self->ptr[self->count -= 1];
		}

		public static void *Get(Lstp *self, int idx) {
			return self->ptr[idx];
		}

		public static string Str(Lstp *self) {
			return string.Format("ptrlst({0}, {1}, 0x{2:X})", self->count, self->len, (int)self->ptr);
		}
	}
}