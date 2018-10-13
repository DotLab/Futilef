using System.Collections.Generic;

namespace Futilef {
	#if FDB
	public static class Fdb {
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
	}
	#endif
}

