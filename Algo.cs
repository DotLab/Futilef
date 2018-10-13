namespace Futilef {
	public static unsafe class Algo {
		public delegate int Cmp(void *a, void *b);
		public static void Qsort(void **arr, int len, Cmp cmp) {
			Qsort(arr, 0, len - 1, cmp);
		}

		public static void Qsort(void **arr, int low, int high, Cmp cmp) {
			if (low < high) {
				int i = low - 1; var pivot = arr[high]; void *swap;
				for (int j = low; j < high; j += 1) {
					if (cmp(arr[j], pivot) <= 0) {
						i += 1;
						swap = arr[i]; arr[i] = arr[j]; arr[j] = swap;
					}
				}
				i += 1;
				swap = arr[i]; arr[i] = arr[high]; arr[high] = swap;
				Qsort(arr, low, i - 1, cmp); 
				Qsort(arr, i + 1, high, cmp);
			}
		}

		#if DEBUG
		public static void Test() {
			Fdb.Log("Algo");

			const int len = 100;
			var arr = stackalloc void *[len];
			for (int i = 0; i < len; i += 1) {
				arr[i] = (void *)Fdb.Random(-len, len);
			}
			Qsort(arr, len, (a, b) => (int)a - (int)b);
			for (int i = 1; i < len; i += 1) {
				Should.BeLessThanOrEqualTo("(int)arr[i - 1]", (int)arr[i - 1], (int)arr[i]);
			}
		}
		#endif
	}
}

