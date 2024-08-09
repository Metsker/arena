using System;
using System.Collections;
using System.Reflection;

namespace Assemblies.Utilities
{
    public class ContentComparer
    {
        public static string Compare(object obj1, object obj2, string path = "")
        {
            if (obj1 == null)
                return path + ": first object is null";
            if (obj2 == null)
                return path + ": second object is null";

            if (!obj1.GetType().Equals(obj2.GetType()))
                return "different types: " + obj1.GetType() + " and " + obj2.GetType();

            Type type = obj1.GetType();
            if (path == "")
                path = type.Name;

            if (type.IsPrimitive || typeof(string).Equals(type))
            {
                if (!obj1.Equals(obj2))
                    return path + ": '" + obj1 + "' != '" + obj2 + "'";
                return null;
            }
            if (type.IsArray)
            {
                Array first = obj1 as Array;
                Array second = obj2 as Array;
                if (first.Length != second.Length)
                    return path + ": array size differs (" + first.Length + " vs " + second.Length + ")";

                var en = first.GetEnumerator();
                int i = 0;
                while (en.MoveNext())
                {
                    string res = Compare(en.Current, second.GetValue(i), path);
                    if (res != null)
                        return res + " (Index " + i + ")";
                    i++;
                }
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                IEnumerable first = obj1 as IEnumerable;
                IEnumerable second = obj2 as IEnumerable;

                var en = first.GetEnumerator();
                var en2 = second.GetEnumerator();
                int i = 0;
                while (en.MoveNext())
                {
                    if (!en2.MoveNext())
                        return path + ": enumerable size differs";

                    string res = Compare(en.Current, en2.Current, path);
                    if (res != null)
                        return res;
                    i++;
                }
            }
            else
            {
                foreach (PropertyInfo pi in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
                {
                    try
                    {
                        var val = pi.GetValue(obj1);
                        var tval = pi.GetValue(obj2);
                        var path1 = (path.Length == 0 ? "" : path + ".") + pi.Name;
                        string res = Compare(val, tval, path1);
                        if (res != null)
                            return res;
                    }
                    catch (TargetParameterCountException)
                    {
                        //index property
                    }
                }
                foreach (FieldInfo fi in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
                {
                    var val = fi.GetValue(obj1);
                    var tval = fi.GetValue(obj2);
                    var path1 = (path.Length == 0 ? "" : path + ".") + fi.Name;
                    string res = Compare(val, tval, path1);
                    if (res != null)
                        return res;
                }
            }
            return null;
        }
    }
}
