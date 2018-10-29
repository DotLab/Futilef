namespace Futilef {
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	public unsafe struct Dict2 {
		#if FDB
		public static int Type = Fdb.NewType("Dict2");
		public int type;
		#endif

		static int[] Lens;
		static int EntrySize, IntSize;
		static bool Initialized;

		public static void TypeInit() {
			Lens = new [] { 3, 7, 13, 31, 61, 127, 251, 509, 1021, 2039, 4093, 8191, 16381, 32749, 65521, 131071, 262139, 524287, 1048573, 2097143, 4194301, 8388593, 16777213, 33554393, 67108859, 134217689, 268435399, 536870909, 1073741789, 2147483647 };
			EntrySize = sizeof(Entry);
			IntSize = sizeof(int);
			Initialized = true;
		}

		public unsafe struct Entry {
			public uint hash;
			public int next;
			public void *key;
			public void *val;
		}

		public int level, len, count, free;
		public Entry *entries;
		public int *arr;

		public static void Init(Dict2 *self) {
			if (!Initialized) TypeInit();
			#if FDB
			Should.NotNull("self", self);
			self->type = Type;
			#endif
			int level = self->level = 0;
			int len = self->len = Lens[level];
			self->count = 0;
			self->free = 0;
			// | (Entry)entries... | (int)arr... |
			// used entry: next: next entry in chain with the collided hash
			//             key: not null
			// free entry: next: next free entry
			//             key: null
			var entries = self->entries = (Entry *)Mem.Malloc(len * (EntrySize + IntSize));
			Entry *entry = null;
			for (int i = 0; i < len; i += 1) {
				entry = entries + i;
				entry->key = null;
				entry->next = i + 1;
			}
			entry->next = -1;  // the last free entry
			int *arr = self->arr = (int *)((byte *)entries + len * EntrySize);
			for (int i = 0; i < len; i += 1) arr[i] = -1;
		}

		public static void *Get(Dict2 *self, uint hash, void *key, Algo.Eq eq) {
			#if FDB
			Verify(self);
			Should.NotNull("key", key);
			Should.NotNull("eq", eq);
			#endif
			int cur = self->arr[hash % self->len];
			var entries = self->entries;
			while (cur != -1) {
				var entry = entries + cur;
				if (entry->hash == hash && eq(entry->key, key)) {  // found the key
					return entry->val;
				}
				cur = entry->next;
			}
			return null;
		}

		public static bool Contains(Dict2 *self, uint hash, void *key, Algo.Eq eq) {
			#if FDB
			Verify(self);
			Should.NotNull("key", key);
			Should.NotNull("eq", eq);
			#endif
			int cur = self->arr[hash % self->len];
			var entries = self->entries;
			while (cur != -1) {
				var entry = entries + cur;
				if (entry->hash == hash && eq(entry->key, key)) {  // found the key
					return true;
				}
				cur = entry->next;
			}
			return false;
		}

		public static void Add(Dict2 *self, uint hash, void *key, void *val, Algo.Eq eq) {
			#if FDB
			Verify(self);
			Should.NotNull("key", key);
			Should.NotNull("eq", eq);
			#endif
			Entry *entry = null; 
			int *arr = self->arr;
			long mod = hash % self->len;
			int idx = arr[mod], cur = idx;
			var entries = self->entries;
			while (cur != -1) {
				entry = entries + cur;
				if (entry->hash == hash && eq(entry->key, key)) {  // found the key
					entry->val = val;
					#if FDB
					Verify(self);
					#endif
					return;
				}
				cur = entry->next;
			}

			// find a free entry
			int free = self->free;
			if (free == -1) {  // expand
				int level = self->level += 1;
				int oldLen = self->len;
				int len = self->len = Lens[level];

				// rehash
				entries = self->entries = (Entry *)Mem.Realloc(entries, len * (EntrySize + IntSize));
				arr = self->arr = (int *)((byte *)entries + len * EntrySize);
				for (int i = 0; i < len; i += 1) arr[i] = -1;
				for (int i = 0; i < oldLen; i += 1) entries[i].next = -1;
				for (int i = 0; i < oldLen; i += 1) {  // insert all old entries
					entry = entries + i;
					mod = entry->hash % len;
					entry->next = arr[mod];
					arr[mod] = i;
				}

				// link all new entries
				self->free = oldLen + 1;  // point to new allocated entries
				for (int i = oldLen + 1; i < len; i += 1)  {
					entry = entries + i;
					entry->key = null;
					entry->next = i + 1;
				}
				entry->next = -1;  // the last free entry

				entry = entries + oldLen;
				idx = arr[mod = hash % len];  // different idx after expand
				free = oldLen;
			} else {
				entry = entries + free;
				self->free = entry->next;
			}

			entry->hash = hash;
			entry->next = idx;
			entry->key = key;
			entry->val = val;
			arr[mod] = free;

			self->count += 1;
			#if FDB
			Verify(self);
			#endif
		}

		public static void Remove(Dict2 *self, uint hash, void *key, Algo.Eq eq) {
			#if FDB
			Verify(self);
			Should.NotNull("key", key);
			Should.NotNull("eq", eq);
			#endif
			int *arr = self->arr;
			long mod = hash % self->len;
			int cur = arr[mod], last = -1;
			var entries = self->entries;
			while (cur != -1) {
				var entry = entries + cur;
				if (entry->hash == hash && eq(entry->key, key)) {  // found the key
					if (last != -1) {
						entries[last].next = entry->next;
					} else {  // the first entry
						arr[mod] = entry->next;
					}
					// insert into the front of free list
					entry->key = null;
					entry->next = self->free;
					self->free = cur;
					self->count -= 1;
					#if FDB
					Verify(self);
					#endif
					return;
				}
				last = cur;
				cur = entry->next;
			}
			// do nothing if no key is found 
		}

		#if FDB
		public static void Verify(Dict2 *self) {
			Should.NotNull("self", self);
			Should.InRange("self->level", self->level, 0, Lens.Length - 1);
			Should.Equal("self->len", self->len, Lens[self->level]);
			Should.InRange("self->count", self->count, 0, self->len);
			Should.InRange("self->free", self->free, -1, self->len - 1);
			Mem.Verify(self->entries);
			Should.NotNull("self->arr", self->arr);

			int len = self->len;
			int count = self->count;
			void **keys = stackalloc void *[count];
			int j = 0;
			for (int i = 0; i < len; i += 1) {
				var entry = self->entries + i;
				if (entry->key != null) {  // a real key
					keys[j++] = entry->key;
				}
			}
			Should.Equal("j", j, count);

			int freeCount = len - count;
			int free = self->free;
			while (free != -1) {
				freeCount -= 1;
				free = self->entries[free].next;
			}
			Should.Zero("freeCount", freeCount);

			j = 0;
			for (int i = 0; i < len; i += 1) {
				int idx = self->arr[i];
				while (idx != -1) {
					var entry = self->entries + idx;
					Should.NotNull("entry->key", entry->key);
					Should.Equal("entry->hash % len", ((uint)entry->hash) % len, i);
					j += 1;
					idx = entry->next;
				}
			}
//			Fdb.Dump(self->arr, self->len * IntSize);
//			Fdb.Dump(self->entries, self->len * EntrySize, EntrySize);
			Should.Equal("j", j, count);
		}

		public static void Test() {
			TestBasic();
			TestSetGet();
			TestSetGetRemove();
			TestRandomSetGetRemove();
		}

		static void TestBasic() {
			var dict = stackalloc Dict2[1]; Init(dict);
			Add(dict, 1, (void *)1, (void *)1, Eq);
			Should.Equal("(int)Get(dict, (void *)1, 1, Eq)", (int)Get(dict, 1, (void *)1, Eq), 1);
			Remove(dict, 1, (void *)1, Eq);
			Should.Null("Get(dict, (void *)1, 1, Eq)", Get(dict, 1, (void *)1, Eq));
		}

		static void TestSetGet() {
			var dict = stackalloc Dict2[1]; Init(dict);
			for (uint i = 2; i < 100; i += 1) {
				Add(dict, i, (void *)i, (void *)i, Eq);
				for (uint j = 2; j <= i; j += 1) {
					Should.Equal("Get(dict, j, (void *)j, Eq)", (uint)Get(dict, j, (void *)j, Eq), j);
				}
			}
		}

		static void TestSetGetRemove() {
			var dict = stackalloc Dict2[1]; Init(dict);
			for (uint i = 2; i < 100; i += 1) {
				Add(dict, i, (void *)i, (void *)i, Eq);
			}
			for (uint i = 2; i < 100; i += 1) {
				Remove(dict, i, (void *)i, Eq);
				for (uint j = 2; j < 100; j += 1) {
					if (j <= i) {
						Should.Null("Get(dict, j, (void *)j, Eq)", Get(dict, j, (void *)j, Eq));
					} else {
						Should.Equal("Get(dict, j, (void *)j, Eq)", (uint)Get(dict, j, (void *)j, Eq), j);			
					}
				}
			}
		}

		static void TestRandomSetGetRemove() {
			var dict = stackalloc Dict2[1]; Init(dict);
			var keyList = new System.Collections.Generic.List<uint>();
			var valList = new System.Collections.Generic.List<uint>();
			for (uint i = 2; i < 100; i += 1) {
				uint key = (uint)Fdb.Random(-0x8000000, 0x7fffffff);
				uint val = (uint)Fdb.Random(-0x8000000, 0x7fffffff);
				Add(dict, key, (void *)key, (void *)val, Eq);
				keyList.Add(key);
				valList.Add(val);
				for (int j = 0; j < keyList.Count; j += 1) {
					Should.Equal("Get(dict, j, (void *)j, Eq)", (uint)Get(dict, keyList[j], (void *)keyList[j], Eq), valList[j]);	
				}
			}
			for (int i = 0; i < keyList.Count; i += 1) {
				Remove(dict, keyList[i], (void *)keyList[i], Eq);
				for (int j = 0; j < keyList.Count; j += 1) {
					if (j <= i) {
						Should.Null("Get(dict, j, (void *)j, Eq)", Get(dict, keyList[j], (void *)keyList[j], Eq));
					} else {
						Should.Equal("Get(dict, j, (void *)j, Eq)", (uint)Get(dict, keyList[j], (void *)keyList[j], Eq), valList[j]);			
					}
				}
			}
		}

		static bool Eq(void *a, void *b) {
			return a == b;
		}
		#endif
	}
}