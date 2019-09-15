namespace DbToRest.Core.Keywords
{
    public static class CoreSystemKeywords
    {
        public static class GeneralKeywords
        {
            public static string HomeRegions { get { return ""; } }
        }

        public static class CacheRepositoryKeywords
        {
            public static string Table { get { return "table:"; } }
            public static string Region { get { return "DbToRest:"; } }
            public static string Define { get { return "define:{0}"; } }
            public static string Method { get { return "{0}:method".FormatWith(string.Concat(Region, ":{0}")); } }
        }
    }
}