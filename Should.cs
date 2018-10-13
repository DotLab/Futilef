using Ptr = System.IntPtr;

namespace Futilef {
	#if FDB
	public static unsafe class Should {
		const string TrueFmt =                 "{0} ({1}) should be true";
		const string FalseFmt =                "{0} ({1}) should be false";
		const string LessThanFmt =             "{0} ({1}) should be less than {2}";
		const string LessThanOrEqualToFmt =    "{0} ({1}) should be less than or equal to {2}";
		const string GreaterThanFmt =          "{0} ({1}) should be greater than {2}";
		const string GreaterThanOrEqualToFmt = "{0} ({1}) should be greater than or equal to {2}";
		const string InRangeFmt =              "{0} ({1}) should be in range {2} ~ {3}";

		const string EqualFmt =                  "{0} ({1}) should equal {2}";
		const string TypeCheckFmt =              "{0} : {1} ({2}) should be an instance of {3}";
		const string NotNullFmt =                "{0} ({1}) should not be null";
		const string NotNullOrEmptyFmt =         "{0} \"{1}\" should not be null or empty";

		public static void True(string o, bool v)  { if (!v) Fdb.AssertionFail(TrueFmt, o, v); }
		public static void False(string o, bool v) { if (v) Fdb.AssertionFail(FalseFmt, o, v); }

		public static void LessThan(string o, int v1, int v2)                 { if (v1 >= v2) Fdb.AssertionFail(LessThanFmt, o, v1, v2); }
		public static void LessThan(string o, float v1, float v2)             { if (v1 >= v2) Fdb.AssertionFail(LessThanFmt, o, v1, v2); }
		public static void LessThanOrEqualTo(string o, int v1, int v2)        { if (v1 > v2) Fdb.AssertionFail(LessThanOrEqualToFmt, o, v1, v2); }
		public static void LessThanOrEqualTo(string o, float v1, float v2)    { if (v1 > v2) Fdb.AssertionFail(LessThanOrEqualToFmt, o, v1, v2); }
		public static void GreaterThan(string o, int v1, int v2)              { if (v1 <= v2) Fdb.AssertionFail(GreaterThanFmt, o, v1, v2); }
		public static void GreaterThan(string o, float v1, float v2)          { if (v1 <= v2) Fdb.AssertionFail(GreaterThanFmt, o, v1, v2); }
		public static void GreaterThanOrEqualTo(string o, int v1, int v2)     { if (v1 < v2) Fdb.AssertionFail(GreaterThanOrEqualToFmt, o, v1, v2); }
		public static void GreaterThanOrEqualTo(string o, float v1, float v2) { if (v1 < v2) Fdb.AssertionFail(GreaterThanOrEqualToFmt, o, v1, v2); }
		public static void InRange(string o, int v, int v1, int v2)           { if (v < v1 || v > v2) Fdb.AssertionFail(InRangeFmt, o, v, v1, v2); }
		public static void InRange(string o, long v, long v1, long v2)        { if (v < v1 || v > v2) Fdb.AssertionFail(InRangeFmt, o, v, v1, v2); }

		public static void Equal(string o, int v1, int v2)     { if (v1 != v2) Fdb.AssertionFail(EqualFmt, o, v1, v2); }
		public static void Equal(string o, long v1, long v2)   { if (v1 != v2) Fdb.AssertionFail(EqualFmt, o, v1, v2); }
		public static void Equal(string o, void *v1, void *v2) { if (v1 != v2) Fdb.AssertionFail(EqualFmt, o, new Ptr(v1), new Ptr(v2)); }
		public static void Equal(string o, float v1, float v2) { if (v1 != v2) Fdb.AssertionFail(EqualFmt, o, v1, v2); }

		public static void TypeEqual(string o, int t1, int t2) { if (t1 != t2) Fdb.AssertionFail(TypeCheckFmt, o, Fdb.GetName(t1), t1, Fdb.GetName(t2)); }
		public static void NotNull(string o, void *p) { if (p == null) Fdb.AssertionFail(NotNullFmt, o, new Ptr(p)); }
		public static void NotNull(string o, object p) { if (p == null) Fdb.AssertionFail(NotNullFmt, o, p); }

		public static void NotNullOrEmpty(string o, string s) { if (string.IsNullOrEmpty(s)) Fdb.AssertionFail(NotNullOrEmptyFmt, o, s); }
	}
	#endif
}

