using System;

namespace ADF.Utility
{
    public static class Constants
    {
        public const string Dot = ".";
        public const char DotChar = '.';
        public const string Space = " ";
        public const char SpaceChar =' ';
        public const string AssemblyName = "SqlSugar";
        public const string ReplaceKey = "{662E689B-17A1-4D06-9D27-F29EAB8BC3D6}";
        public const string ReplaceCommaKey = "{112A689B-17A1-4A06-9D27-A39EAB8BC3D5}";

        public static Type IntType = typeof(int);
        public static Type LongType = typeof(long);
        public static Type GuidType = typeof(Guid);
        public static Type BoolType = typeof(bool);
        public static Type BoolTypeNull = typeof(bool?);
        public static Type ByteType = typeof(Byte);
        public static Type ObjType = typeof(object);
        public static Type DobType = typeof(double);
        public static Type FloatType = typeof(float);
        public static Type ShortType = typeof(short);
        public static Type DecType = typeof(decimal);
        public static Type StringType = typeof(string);
        public static Type DateType = typeof(DateTime);
        public static Type DateTimeOffsetType = typeof(DateTimeOffset);
        public static Type TimeSpanType = typeof(TimeSpan);
        public static Type ByteArrayType = typeof(byte[]);

        // public static Type ModelType= typeof(ModelContext);
        // public static Type DynamicType = typeof(ExpandoObject);
        // public static Type Dicii = typeof(KeyValuePair<int, int>);
        // public static Type DicIS = typeof(KeyValuePair<int, string>);
        // public static Type DicSi = typeof(KeyValuePair<string, int>);
        // public static Type DicSS = typeof(KeyValuePair<string, string>);
        // public static Type DicOO = typeof(KeyValuePair<object, object>);
        // public static Type DicSo = typeof(KeyValuePair<string, object>);
        // public static Type DicArraySS = typeof(Dictionary<string, string>);
        // public static Type DicArraySO = typeof(Dictionary<string, object>);
        // public static Type SugarType = typeof(SqlSugarClient);
    }
}