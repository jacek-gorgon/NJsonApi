using System;

namespace NJsonApi.Utils
{
    public static class CamelCaseUtil
    {
        public static string ToCamelCase(string text)
        {
            return Char.ToLowerInvariant(text[0]) + text.Substring(1);
        }
    }
}
