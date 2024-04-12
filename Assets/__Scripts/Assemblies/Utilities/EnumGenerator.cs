#if UNITY_EDITOR
using System.IO;
using UnityEditor;

namespace __Scripts.Assemblies.Utilities
{
    public static class EnumGenerator
    {
        public static void Generate(string enumName, string[] enumEntries)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Generated"))
                AssetDatabase.CreateFolder("Assets", "Generated");
        
            string filePathAndName = "Assets/Generated/" + enumName + ".cs";

            using (StreamWriter streamWriter = new (filePathAndName))
            {
                streamWriter.WriteLine("namespace Generated");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine("    public enum " + enumName);
                streamWriter.WriteLine("    {");
                for (int i = 0; i < enumEntries.Length; i++)
                {
                    string entry = enumEntries[i];
                    streamWriter.WriteLine($"\t    {entry.Replace(" ", string.Empty)} = {i},");
                }
                streamWriter.WriteLine("    }");
                streamWriter.WriteLine("}");
            }
            AssetDatabase.Refresh();
        }
    }
}
#endif
