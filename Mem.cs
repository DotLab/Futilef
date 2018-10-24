using System;
using System.Runtime.InteropServices;


namespace Futilef {
	public unsafe static class Mem {
		public static void *Malloc(int n) {
			#if FDB
			Should.GreaterThan("n", n, 0);
			#endif
			return (void *)Marshal.AllocHGlobal(n);
		}

		public static void Free(void *a) {
			#if FDB
			Should.NotNull("a", a);
			#endif
			Marshal.FreeHGlobal((IntPtr)a);
		}

		public static void *Realloc(void *a, int n) {
			#if FDB
			Should.NotNull("a", a);
			Should.GreaterThan("n", n, 0);
			#endif
			return (void *)Marshal.ReAllocHGlobal((IntPtr)a, (IntPtr)n);
		}

		public static void Memcpy(void *dst, void *src, int n) {
			#if FDB
			Should.Zero("(long)dst % sizeof(int)", (long)dst % sizeof(int), 0);
			Should.Zero("(long)src % sizeof(int)", (long)src % sizeof(int), 0);
			#endif
			#if UNITY_STANDALONE_WIN
			__WinMemcpy((IntPtr)dst, (IntPtr)src, (UIntPtr)(long)n);
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

		#if UNITY_STANDALONE_WIN
		[DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
		static extern IntPtr __WinMemcpy(IntPtr dest, IntPtr src, UIntPtr count);
		#endif
	}
}