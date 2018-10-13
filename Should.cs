using Debug = UnityEngine.Debug;

namespace Futilef {
	#if DEBUG
	public static unsafe class Should {
		const string BeTrueFmt =                 "{0} ({1}) should be true";
		const string BeLessThanFmt =             "{0} ({1}) should be less than {2}";
		const string BeLessThanOrEqualToFmt =    "{0} ({1}) should be less than or equal to {2}";
		const string BeGreaterThanFmt =          "{0} ({1}) should be greater than {2}";
		const string BeGreaterThanOrEqualToFmt = "{0} ({1}) should be greater than or equal to {2}";

		const string EqualFmt =                  "{0} ({1}) should equal {2}";
		const string TypeCheckFmt =              "{0} : {1} ({2}) should be an instance of {3}";
		const string FailFmt =                   "{0} should fail";

		public static void BeTrue(string o, bool v) {
			if (!v) Fdb.Error(BeTrueFmt, o, v);
		}

		public static void BeLessThan(string o, int v1, int v2)                 { if (v1 >= v2) Fdb.Error(BeLessThanFmt, o, v1, v2); }
		public static void BeLessThan(string o, float v1, float v2)             { if (v1 >= v2) Fdb.Error(BeLessThanFmt, o, v1, v2); }
		public static void BeLessThanOrEqualTo(string o, int v1, int v2)        { if (v1 > v2) Fdb.Error(BeLessThanOrEqualToFmt, o, v1, v2); }
		public static void BeLessThanOrEqualTo(string o, float v1, float v2)    { if (v1 > v2) Fdb.Error(BeLessThanOrEqualToFmt, o, v1, v2); }
		public static void BeGreaterThan(string o, int v1, int v2)              { if (v1 <= v2) Fdb.Error(BeGreaterThanFmt, o, v1, v2); }
		public static void BeGreaterThan(string o, float v1, float v2)          { if (v1 <= v2) Fdb.Error(BeGreaterThanFmt, o, v1, v2); }
		public static void BeGreaterThanOrEqualTo(string o, int v1, int v2)     { if (v1 < v2) Fdb.Error(BeGreaterThanOrEqualToFmt, o, v1, v2); }
		public static void BeGreaterThanOrEqualTo(string o, float v1, float v2) { if (v1 < v2) Fdb.Error(BeGreaterThanOrEqualToFmt, o, v1, v2); }

		public static void Equal(string o, int v1, int v2)     { if (v1 != v2) Fdb.Error(EqualFmt, o, v1, v2); }
		public static void Equal(string o, long v1, long v2)   { if (v1 != v2) Fdb.Error(EqualFmt, o, v1, v2); }
		public static void Equal(string o, void *v1, void *v2) { if (v1 != v2) Fdb.Error(EqualFmt, o, (ulong)v1, (ulong)v2); }
		public static void Equal(string o, float v1, float v2) { if (v1 != v2) Fdb.Error(EqualFmt, o, v1, v2); }

		public static void TypeCheck(string o, int t1, int t2) {
			if (t1 != t2) Fdb.Error(TypeCheckFmt, o, Fdb.GetName(t1), t1, Fdb.GetName(t2));
		}

		public static void Fail(string o) { Fdb.Error(FailFmt, o); }
	}
	#endif
}

