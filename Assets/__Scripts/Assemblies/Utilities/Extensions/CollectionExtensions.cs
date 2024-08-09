using System.Collections.Generic;

namespace Assemblies.Utilities.Extensions
{
    public static class CollectionExtensions
    {
        public static void Replace<T>(this List<T> list, T oldValue, T newValue)
        {
            int index = list.IndexOf(oldValue);
            if(index != -1)
                list[index] = newValue;
        }
    }
}
