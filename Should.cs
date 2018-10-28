﻿using Ptr = System.IntPtr;

namespace Futilef {
	#if FDB
	public static unsafe class Should {
		const string TrueErr =                 "{0} ({1}) should be true";
		const string TrueLog =                 "{0} ({1}) is true";
		const string FalseErr =                "{0} ({1}) should be false";
		const string FalseLog =                "{0} ({1}) is false";
		const string LessThanErr =             "{0} ({1}) should be less than {2}";
		const string LessThanLog =             "{0} ({1}) is less than {2}";
		const string LessThanOrEqualToErr =    "{0} ({1}) should be less than or equal to {2}";
		const string LessThanOrEqualToLog =    "{0} ({1}) is less than or equal to {2}";
		const string GreaterThanErr =          "{0} ({1}) should be greater than {2}";
		const string GreaterThanLog =          "{0} ({1}) is greater than {2}";
		const string GreaterThanOrEqualToErr = "{0} ({1}) should be greater than or equal to {2}";
		const string GreaterThanOrEqualToLog = "{0} ({1}) is greater than or equal to {2}";
		const string InRangeErr =              "{0} ({1}) should be in range {2} ~ {3}";
		const string InRangeLog =              "{0} ({1}) is in range {2} ~ {3}";

		const string EqualErr =                "{0} ({1}) should equal {2}";
		const string EqualLog =                "{0} ({1}) equals {2}";
		const string TypeCheckErr =            "{0} : {1} ({2}) should be an instance of {3}";
		const string TypeCheckLog =            "{0} : {1} ({2}) is an instance of {3}";
		const string NullErr =                 "{0} ({1}) should be null";
		const string NullLog =                 "{0} ({1}) is null";
		const string NotNullErr =              "{0} ({1}) should not be null";
		const string NotNullLog =              "{0} ({1}) is not null";
		const string NotNullOrEmptyErr =       "{0} \"{1}\" should not be null or empty";
		const string NotNullOrEmptyLog =       "{0} \"{1}\" is not null or empty";

		public static void True(string o, bool v)  { if (!v) Fdb.AssertionFail(TrueErr, o, v); }
		public static void False(string o, bool v) { if (v) Fdb.AssertionFail(FalseErr, o, v); }

		public static void LessThan(string o, int v1, int v2)                 { if (v1 >= v2) Fdb.AssertionFail(LessThanErr, o, v1, v2); }
		public static void LessThan(string o, float v1, float v2)             { if (v1 >= v2) Fdb.AssertionFail(LessThanErr, o, v1, v2); }
		public static void LessThanOrEqualTo(string o, int v1, int v2)        { if (v1 > v2) Fdb.AssertionFail(LessThanOrEqualToErr, o, v1, v2); }
		public static void LessThanOrEqualTo(string o, float v1, float v2)    { if (v1 > v2) Fdb.AssertionFail(LessThanOrEqualToErr, o, v1, v2); }
		public static void GreaterThan(string o, int v1, int v2)              { if (v1 <= v2) Fdb.AssertionFail(GreaterThanErr, o, v1, v2); }
		public static void GreaterThan(string o, float v1, float v2)          { if (v1 <= v2) Fdb.AssertionFail(GreaterThanErr, o, v1, v2); }
		public static void GreaterThanOrEqualTo(string o, int v1, int v2)     { if (v1 < v2) Fdb.AssertionFail(GreaterThanOrEqualToErr, o, v1, v2); }
		public static void GreaterThanOrEqualTo(string o, float v1, float v2) { if (v1 < v2) Fdb.AssertionFail(GreaterThanOrEqualToErr, o, v1, v2); }
		public static void InRange(string o, int v, int v1, int v2)           { if (v < v1 || v > v2) Fdb.AssertionFail(InRangeErr, o, v, v1, v2); }
		public static void InRange(string o, long v, long v1, long v2)        { if (v < v1 || v > v2) Fdb.AssertionFail(InRangeErr, o, v, v1, v2); }
		public static void InRange(string o, void *v, void *v1, void *v2)     { if (v < v1 || v > v2) Fdb.AssertionFail(InRangeErr, o, (long)v, (long)v1, (long)v2); }

		public static void GreaterThanZero(string o, int v)                   { if (v <= 0) Fdb.AssertionFail(GreaterThanErr, o, v, "zero"); }
		public static void EqualZero(string o, int v)                         { if (v != 0) Fdb.AssertionFail(EqualErr, o, v, 0); }

		public static void Equal(string o, int v1, int v2)     { if (v1 != v2) Fdb.AssertionFail(EqualErr, o, v1, v2); }
		public static void Equal(string o, long v1, long v2)   { if (v1 != v2) Fdb.AssertionFail(EqualErr, o, v1, v2); }
		public static void Equal(string o, void *v1, void *v2) { if (v1 != v2) Fdb.AssertionFail(EqualErr, o, new Ptr(v1), new Ptr(v2)); }
		public static void Equal(string o, float v1, float v2) { if (v1 != v2) Fdb.AssertionFail(EqualErr, o, v1, v2); }

		public static void TypeEqual(string o, int t1, int t2) { if (t1 != t2) Fdb.AssertionFail(TypeCheckErr, o, Fdb.GetName(t1), t1, Fdb.GetName(t2)); }
		public static void Null(string o, void *p) { if (p != null) Fdb.AssertionFail(NullErr, o, new Ptr(p)); }
		public static void NotNull(string o, void *p) { if (p == null) Fdb.AssertionFail(NotNullErr, o, new Ptr(p)); }
		public static void NotNull(string o, object p) { if (p == null) Fdb.AssertionFail(NotNullErr, o, p); }

		public static void NotNullOrEmpty(string o, string s) { if (string.IsNullOrEmpty(s)) Fdb.AssertionFail(NotNullOrEmptyErr, o, s); }
	}
	#endif
}

