using IntPtr = System.IntPtr;
using Marshal = System.Runtime.InteropServices.Marshal;

namespace Futilef {
	public unsafe static class Mem {
		public static void *Alloc(int n) {
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
	}
}