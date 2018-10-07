using System.Runtime.InteropServices;

namespace Futilef {
	public unsafe static class Mem {
		public static void *Alloc(int n) {
			return (void *)Marshal.AllocHGlobal(n);
		}

		public static void Free(void *a) {
			Marshal.FreeHGlobal((System.IntPtr)a);
		}
	}
}