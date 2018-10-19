using System.Collections.Generic;

namespace Futilef {
	#if FDB
	public static unsafe class Fdb {
		class FdbError : System.Exception { public FdbError(string msg) : base(msg) {} }
		class FdbAssertionFail : System.Exception { public FdbAssertionFail(string msg) : base(msg) {} }

		public const int NullType = -1;
		const int TypeOffset = 100;

		static readonly List<string> typeList = new List<string>();

		static Fdb() {
			System.Reflection.Assembly
				.GetAssembly(typeof(UnityEditor.SceneView))
				.GetType("UnityEditor.LogEntries")
				.GetMethod("Clear")
				.Invoke(new object(), null);

			var sw = new System.Diagnostics.Stopwatch();
			sw.Reset(); sw.Start();
			Algo.Test();
			Log("Algo Test: {0:N0}", sw.ElapsedTicks); sw.Reset(); sw.Start();
			Lst.Test();
			Log("Lst Test: {0:N0}", sw.ElapsedTicks); sw.Reset(); sw.Start();
			PtrLst.Test();
			Log("PtrLst Test: {0:N0}", sw.ElapsedTicks); sw.Reset(); sw.Start();
			Pool.Test();
			Log("Pool Test: {0:N0}", sw.ElapsedTicks); sw.Reset(); sw.Start();
			Dict.Test();
			Log("Dict Test: {0:N0}", sw.ElapsedTicks);
		}

		public static int NewType(string name) {
			int type = typeList.Count + TypeOffset;
			typeList.Add(name);
			return type;
		}

		public static string GetName(int type) {
			if (type == NullType) return "null";
			type -= TypeOffset;
			if (0 <= type && type < typeList.Count) { 
				return typeList[type];
			}
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

		public static string Dump(byte *ptr, int size, int ncol = 16) {
			var sb = new System.Text.StringBuilder();
			sb.AppendFormat("{0} bytes at 0x{1:X}\n", size, (long)ptr);
			for (int i = 0; i < size; i += 1) {
				if (i % ncol == 0) sb.AppendFormat("{0:X8}: ", (long)ptr);
				sb.AppendFormat("{0:X2}", *ptr++);				
				if ((i + 1) % 4 == 0) sb.Append(" ");
				if ((i + 1) % ncol == 0) sb.AppendLine();
			}
			string str = sb.ToString();
			Log(str);
			return str;
		}
	}
	#endif
}

