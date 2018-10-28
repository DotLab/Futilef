using System;
using System.Runtime.InteropServices;


namespace Futilef {
	public unsafe static class Mem {
		#if FDB
		public static readonly int Type = Fdb.NewType("Mem");

		// | (int)len | (byte)mem... | (int)Type |
		public static int Verify(void *mem) {
			Should.NotNull("mem", mem);
			int len = *(int *)((byte *)mem - sizeof(int));
			Should.GreaterThanZero("*(int *)((byte *)mem - sizeof(int))", len);
			int type = *(int *)((byte *)mem + len);
			Should.TypeEqual("*(int *)((byte *)mem + len)", type, Type);
			return len;
		}
		#endif

		public static void *Malloc(int n) {
			#if FDB
			Should.GreaterThanZero("n", n);
			byte *p = (byte *)Marshal.AllocHGlobal(n + 2 * sizeof(int));
			*(int *)p = n;
			*(int *)(p + sizeof(int) + n) = Type;
			return p + sizeof(int);
			#else
			return (void *)Marshal.AllocHGlobal(n);
			#endif
		}

		public static void Free(void *mem) {
			#if FDB
			Verify(mem);
			Marshal.FreeHGlobal((IntPtr)((byte *)mem - sizeof(int)));
			#else 
			Marshal.FreeHGlobal((IntPtr)mem);
			#endif
		}

		public static void *Realloc(void *mem, int n) {
			#if FDB
			Verify(mem);
			Should.GreaterThan("n", n, 0);
			byte *p = (byte *)Marshal.ReAllocHGlobal((IntPtr)((byte *)mem - sizeof(int)), (IntPtr)(n + 2 * sizeof(int)));
			*(int *)p = n;
			*(int *)(p + sizeof(int) + n) = Type;
			return p + sizeof(int);
			#else
			return (void *)Marshal.ReAllocHGlobal((IntPtr)mem, (IntPtr)n);
			#endif
		}

		public static void Memcpy(void *dst, void *src, int n) {
			#if FDB
//			Should.Zero("(long)dst % sizeof(int)", (long)dst % sizeof(int), 0);
//			Should.Zero("(long)src % sizeof(int)", (long)src % sizeof(int), 0);
			#endif
			#if UNITY_STANDALONE_WIN
			__memcpy((IntPtr)dst, (IntPtr)src, (UIntPtr)(long)n);
			#else
			if (n % sizeof(long) == 0) {
				var d = (long *)dst; var s = (long *)src;
				n /= sizeof(long);
				while (n-- > 0) *d++ = *s++;
			} else {
				var d = (byte *)dst; var s = (byte *)src;
				while (n-- > 0) *d++ = *s++;
			}
			#endif
		}

		public static void Memmove(void *dst, void *src, int n) {
			#if UNITY_STANDALONE_WIN
			__memmove((IntPtr)dst, (IntPtr)src, (UIntPtr)(long)n);
			#else
			var d = (byte *)dst; var s = (byte *)src;
			while (n-- > 0) *d++ = *s++;
			#endif
		}

		#if UNITY_STANDALONE_WIN
		[DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
		static extern IntPtr __memcpy(IntPtr dest, IntPtr src, UIntPtr count);
		[DllImport("msvcrt.dll", EntryPoint = "memmove", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
		static extern IntPtr __memmove(IntPtr dest, IntPtr src, UIntPtr count);
		#endif

		#if FDB
		public static void Test() {
			void *a = Malloc(64);
			Should.Equal("Verify(a)", Verify(a), 64);
			*(int *)a = 100;
			a = Realloc(a, 128);
			Should.Equal("Verify(a)", Verify(a), 128);
			Should.Equal("*(int *)a", *(int *)a, 100);
			Free(a);
		}
		#endif
	}
}