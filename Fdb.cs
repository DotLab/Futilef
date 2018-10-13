using System.Collections.Generic;

namespace Futilef {
	#if FDB
	public static unsafe class Fdb {
		class FdbError : System.Exception { public FdbError(string msg) : base(msg) {} }
		class FdbAssertionFail : System.Exception { public FdbAssertionFail(string msg) : base(msg) {} }

		public const int NullType = -1;

		static readonly List<string> typeList = new List<string>();

		static Fdb() {
			System.Reflection.Assembly
				.GetAssembly(typeof(UnityEditor.SceneView))
				.GetType("UnityEditor.LogEntries")
				.GetMethod("Clear")
				.Invoke(new object(), null);

			Log("Algo Test"); Algo.Test();
			Log("Lst Test"); Lst.Test();
			Log("PtrLst Test"); PtrLst.Test();
			Log("Pool Test"); Pool.Test();
		}

		public static int NewType(string name) {
			typeList.Add(name);
			return typeList.Count - 1;
		}

		public static string GetName(int type) {
			if (type == NullType) return "null";
			if (type < typeList.Count) return typeList[type];
			return "?";
		}

		public static int Random(int start, int end) {
			return UnityEngine.Random.Range(start, end);
		}

		public static void Log(string fmt, params object[] args) {
			UnityEngine.Debug.LogFormat("Fdb: " + fmt, args);
		}

		public static void Error(string fmt, params object[] args) {
			throw new FdbError(string.Format(fmt, args));
		}

		public static void AssertionFail(string fmt, params object[] args) {
			throw new FdbAssertionFail(string.Format(fmt, args));
		}

		public static void Dump(byte *ptr, int size, int ncol = 16) {
			var sb = new System.Text.StringBuilder();
			sb.AppendFormat("{0} bytes at 0x{1:X}\n", size, (long)ptr);
			for (int i = 0; i < size; i += 1) {
				if (i % ncol == 0) sb.AppendFormat("{0:X8}: ", (long)ptr);
				sb.AppendFormat("{0:X2}", *ptr++);				
				if ((i + 1) % 4 == 0) sb.Append(" ");
				if ((i + 1) % ncol == 0) sb.AppendLine();
			}
			Log(sb.ToString());
		}
	}
	#endif
}

