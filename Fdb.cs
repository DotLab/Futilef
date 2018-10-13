using System.Collections.Generic;

namespace Futilef {
	#if DEBUG
	public static unsafe class Fdb {
		public const int NullType = -1;

		static readonly List<string> typeList = new List<string>();

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
			UnityEngine.Debug.LogErrorFormat("Fdb: " + fmt, args);
		}
	}
	#endif
}

