using IntPtr = System.IntPtr;
using Marshal = System.Runtime.InteropServices.Marshal;

namespace Futilef {
	public unsafe static class Mem {
		public static void *Alloc(int n) {
			return (void *)Marshal.AllocHGlobal(n);
		}

		public static void Free(void *a) {
			Marshal.FreeHGlobal((IntPtr)a);
		}

		public static void *Realloc(void *a, int n) {
			return (void *)Marshal.ReAllocHGlobal((IntPtr)a, (IntPtr)n);
		}
	}
}