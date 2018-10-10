using UnityEngine;

namespace Futilef {
	public static unsafe class Test {
		public static void QsortTest() {
			var lst = PtrLst.New();
			for (int i = 0; i < 100; i += 1) {
				int r = Random.Range(0, 100);
				PtrLst.Push(lst, (void *)r);
			}
			PtrLst.Qsort(lst, (a, b) => (int)a - (int)b);
			for (int i = 0; i < 100; i += 1) {
				Debug.Log((int)lst->arr[i]);
			}
		}

		public static void LstTest() {
			var lst = stackalloc Lst[1];
			int t = 100;
			Lst.Init(lst, sizeof(int));
			Lst.Push(lst, &t);
			t = 200;
			Lst.Push(lst, &t);
			Lst.RemoveAt(lst, 0);
			Debug.Log(Lst.Str(lst));
			Debug.Log(*(int *)lst->arr);
			Lst.Decon(lst);
		}

		public static void PtrLstTest() {
			var lst = stackalloc PtrLst[1];
			PtrLst.Init(lst);
			PtrLst.Push(lst, (void *)100);
			PtrLst.Push(lst, (void *)200);
			PtrLst.Remove(lst, (void *)100);
			Debug.Log(PtrLst.Str(lst));
			Debug.Log(*(int *)lst->arr);
			PtrLst.Decon(lst);
		}
	}
}
