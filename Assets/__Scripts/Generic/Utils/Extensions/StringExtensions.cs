using System;
using System.Collections.Generic;
using System.Linq;
namespace Arena.__Scripts.Shared.Utils.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<T> ToEnums<T>(this IEnumerable<string> strings, bool ignoreCase = true) where T : struct =>
            strings.Select(s => Enum.Parse<T>(s, ignoreCase));
    }
}
