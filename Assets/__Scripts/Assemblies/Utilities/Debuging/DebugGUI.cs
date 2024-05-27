#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace __Scripts.Assemblies.Utilities.Debuging
{
    [InitializeOnLoad]
    public class DebugGUI
    {
        private static readonly DebugData DebugData;

        static DebugGUI()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(DebugData)}");
            
            if (guids.Length <= 0)
            {
                DebugData = ScriptableObject.CreateInstance<DebugData>();
                Debug.Log(typeof(DebugData) + " created");
                return;
            }
            
            DebugData = AssetDatabase.LoadAssetAtPath<DebugData>(AssetDatabase.GUIDToAssetPath(guids[0]));

            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        }
        
        private static void OnToolbarGUI()
        {
            DebugData.isDebug = GUILayout.Toggle(DebugData.isDebug,
                new GUIContent("Debug", "Switch debug mode"));
        }
    }
}
#endif
