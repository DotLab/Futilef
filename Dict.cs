namespace Futilef {
	public unsafe struct Dict {
		static readonly int[] Lens   = { 3, 7, 13, 31, 61, 127, 251, 509, 1021, 2039, 4093, 8191, 16381, 32749, 65521, 131071, 262139, 524287, 1048573, 2097143, 4194301, 8388593, 16777213, 33554393, 67108859, 134217689, 268435399, 536870909, 1073741789, 2147483647 };
		static readonly uint[] Prims = { 2, 5, 11, 29, 59, 113, 241, 503, 1019, 2029, 4091, 8179, 16369, 32719, 65519, 131063, 262133, 524269, 1048571, 2097133, 4194287, 8388587, 16777199, 33554383, 67108837, 134217649, 268435367, 536870879, 1073741783, 2147483629 };

		const int EmptyHash = 0x4a4a4a4a;
		const int DeletedKey = 1;
		const float LoadFactor = .7f;

		static readonly int Size = sizeof(uint) + (sizeof(void *) << 1);
		static readonly int HashSize = sizeof(uint);
		static readonly int HashKeySize = sizeof(uint) + sizeof(void *);

		#if FDB
		static readonly int Type = Fdb.NewType("Dict");
		int type;
		#endif

		public int count, len;

		// | (uint) hash | (void *)key | (void *)val |
		public byte *arr;

		int lv;
		uint prim;
		int limit;

		public static Dict *Init(Dict *self) {
			#if FDB
			Should.NotNull("self", self);
			self->type = Type;
			#endif
			self->count = 0;
			int lv = self->lv = 1;
			int len = self->len = Lens[lv];
			self->prim = Prims[lv];
			self->limit = (int)(len * LoadFactor);

			var arr = self->arr = (byte *)Mem.Malloc(len * Size);
			for (int i = 0, end = len * Size; i < end; i += Size) {
				var hashPtr = arr + i;
				*(uint *)hashPtr = EmptyHash;
				*(void **)(hashPtr + HashSize) = null;
				*(void **)(hashPtr + HashKeySize) = null;
			}

			return self;
		}

		public static void Set(Dict *self, uint hash, void *key, void *val, Algo.Eq eq) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			Should.NotNull("key", key);
			Should.NotNull("eq", eq);
			int oldCount = self->count;
			int oldLen = self->len;
			#endif
			if ((self->count += 1) > self->limit) {  // expand and rehash
				LevelUp(self);
				#if FDB
				Should.GreaterThan("self->len", self->len, oldLen);
				#endif
			}

			var arr = self->arr; int len = self->len; uint prim = self->prim;
			uint h2 = prim - (hash % prim);
			
			var hashPtr = arr + (hash % len) * Size;
			var keyCur = *(void **)(hashPtr + HashSize);
			byte *deletedHashPtr = null;

//			var str = Fdb.Dump(arr, self->len * Size, Size);

			uint i = h2;
			while (keyCur != null) {
				if (deletedHashPtr == null && keyCur == (void *)DeletedKey) {
					deletedHashPtr = (byte *)hashPtr;
				}

				if (*(uint *)hashPtr == hash && eq(keyCur, key)) {  // found key
					*(void **)(hashPtr + HashKeySize) = val;
					return;
				}
				hashPtr = arr + ((hash + i) % len) * Size; i += h2;
				keyCur = *(void **)(hashPtr + HashSize);
			}

			// new key
			if (deletedHashPtr != null) hashPtr = deletedHashPtr;
			*(uint *)hashPtr = hash;
			*(void **)(hashPtr + HashSize) = key;
			*(void **)(hashPtr + HashKeySize) = val;

//			var str2 = Fdb.Dump(arr, self->len * Size, Size);
			#if FDB
			Should.Equal("self->count", self->count, oldCount + 1);
			#endif
		}

		static void LevelUp(Dict *self) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			#endif
			int lv = self->lv += 1;
			int len = self->len;
			int newLen = self->len = Lens[lv];
			uint prim = self->prim = Prims[lv];
			self->limit = (int)(self->len * LoadFactor);

			var arr = self->arr;
			var newArr = self->arr = (byte *)Mem.Malloc(newLen * Size);
			for (int i = 0, end = newLen * Size; i < end; i += Size) {
				var hashPtr = newArr + i;
				*(uint *)hashPtr = EmptyHash;
				*(void **)(hashPtr + HashSize) = null;
				*(void **)(hashPtr + HashKeySize) = null;
			}

//			var str = Fdb.Dump(arr, len * Size, Size);
			for (int j = 0, end = len * Size; j < end; j += Size) {
				var hashPtr = arr + j;
				var keyCur = *(void **)(hashPtr + HashSize);
				if (keyCur != null && keyCur != (void *)DeletedKey) {  // reinsert
					uint hash = *(uint *)hashPtr, h2 = prim - (hash % prim);

					var newHashPtr = newArr + (hash % newLen) * Size;
					var newKeyCur = *(void **)(newHashPtr + HashSize);

//					var str2 = Fdb.Dump(newArr, newLen * Size, Size);

					uint i = h2;
					while (newKeyCur != null) {  // do not check hash or key eq
						newHashPtr = newArr + ((hash + i) % newLen) * Size; i += h2;
						newKeyCur = *(void **)(newHashPtr + HashSize);
					}

					// new key
					*(uint *)newHashPtr = hash;
					*(void **)(newHashPtr + HashSize) = keyCur;
					*(void **)(newHashPtr + HashKeySize) = *(void **)(hashPtr + HashKeySize);

//					var str3 = Fdb.Dump(newArr, newLen * Size, Size);
				}
			}

			Mem.Free(arr);
		}

		public static void *Get(Dict *self, uint hash, void *key, Algo.Eq eq) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			Should.NotNull("key", key);
			Should.NotNull("eq", eq);
			#endif
			var arr = self->arr; int len = self->len; uint prim = self->prim;
			uint h2 = prim - (hash % prim);

			var hashPtr = arr + (hash % len) * Size;
			var keyCur = *(void **)(hashPtr + HashSize);

			uint i = h2;
			while (keyCur != null) {
				if (*(uint *)hashPtr == hash && eq(keyCur, key)) {  // found key
					return *(void **)(hashPtr + HashKeySize);
				}
				hashPtr = arr + ((hash + i) % len) * Size; i += h2;
				keyCur = *(void **)(hashPtr + HashSize);
			}

			return null;
		}

		public static void Remove(Dict *self, uint hash, void *key, Algo.Eq eq) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			Should.NotNull("key", key);
			Should.NotNull("eq", eq);
			#endif
			var arr = self->arr; int len = self->len; uint prim = self->prim;
			uint h2 = prim - (hash % prim);

			var hashPtr = arr + (hash % len) * Size;
			var keyCur = *(void **)(hashPtr + HashSize);

			uint i = h2;
			while (keyCur != null) {
				if (*(uint *)hashPtr == hash && eq(keyCur, key)) {  // found key
					*(uint *)hashPtr = EmptyHash;
					*(void **)(hashPtr + HashSize) = (void *)DeletedKey;
					*(void **)(hashPtr + HashKeySize) = null;
					self->count -= 1;
					return;
				}
				hashPtr = arr + ((hash + i) % len) * Size; i += h2;
				keyCur = *(void **)(hashPtr + HashSize);
			}
			#if FDB
			Fdb.Error("{0} does not exist in Dict {1}", hash, (ulong) self);
			#endif
		}

		#if FDB
		public static void Test() {
			TestSetGet();
			TestSetGetRemove();
			TestRandomSetGetRemove();
		}

		static void TestSetGet() {
			var dict = stackalloc Dict[1]; Init(dict);
			Algo.Eq eq = (a, b) => (uint)a == (uint)b;
			Should.True("eq(0x100, 0x100)", eq((void *)0x100, (void *)0x100));
			for (uint i = 2; i < 100; i += 1) {
				Dict.Set(dict, i, (void *)i, (void *)i, eq);
				for (uint j = 2; j <= i; j += 1) {
					Should.Equal("Dict.Get(dict, j, (void *)j, eq)", (uint)Dict.Get(dict, j, (void *)j, eq), j);
				}
			}
		}

		static void TestSetGetRemove() {
			var dict = stackalloc Dict[1]; Init(dict);
			Algo.Eq eq = (a, b) => (uint)a == (uint)b;
			for (uint i = 2; i < 100; i += 1) {
				Dict.Set(dict, i, (void *)i, (void *)i, eq);
			}
			for (uint i = 2; i < 100; i += 1) {
				Dict.Remove(dict, i, (void *)i, eq);
				for (uint j = 2; j < 100; j += 1) {
					if (j <= i) {
						Should.Null("Dict.Get(dict, j, (void *)j, eq)", Dict.Get(dict, j, (void *)j, eq));
					} else {
						Should.Equal("Dict.Get(dict, j, (void *)j, eq)", (uint)Dict.Get(dict, j, (void *)j, eq), j);			
					}
				}
			}
		}

		static void TestRandomSetGetRemove() {
			var dict = stackalloc Dict[1]; Init(dict);
			var keyList = new System.Collections.Generic.List<uint>();
			var valList = new System.Collections.Generic.List<uint>();
			Algo.Eq eq = (a, b) => (uint)a == (uint)b;
			for (uint i = 2; i < 100; i += 1) {
				uint key = (uint)Fdb.Random(-0x8000000, 0x7fffffff);
				uint val = (uint)Fdb.Random(-0x8000000, 0x7fffffff);
				Dict.Set(dict, key, (void *)key, (void *)val, eq);
				keyList.Add(key);
				valList.Add(val);
				for (int j = 0; j < keyList.Count; j += 1) {
					Should.Equal("Dict.Get(dict, j, (void *)j, eq)", (uint)Dict.Get(dict, keyList[j], (void *)keyList[j], eq), valList[j]);	
				}
			}
			for (int i = 0; i < keyList.Count; i += 1) {
				Dict.Remove(dict, keyList[i], (void *)keyList[i], eq);
				for (int j = 0; j < keyList.Count; j += 1) {
					if (j <= i) {
						Should.Null("Dict.Get(dict, j, (void *)j, eq)", Dict.Get(dict, keyList[j], (void *)keyList[j], eq));
					} else {
						Should.Equal("Dict.Get(dict, j, (void *)j, eq)", (uint)Dict.Get(dict, keyList[j], (void *)keyList[j], eq), valList[j]);			
					}
				}
			}
		}
		#endif
	}
}

