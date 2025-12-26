namespace System
{
    public static class SystemExtensions
    {
        public static string TrimAll(this string? source)
        {
            if (source == null || source.GetType() == typeof(DBNull))
            {
                return string.Empty;
            }
            return source.TrimEnd().TrimStart();
        }
    }
}
