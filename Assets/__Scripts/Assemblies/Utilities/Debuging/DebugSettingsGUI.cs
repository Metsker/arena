#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace Assemblies.Utilities.Debuging
{
    [InitializeOnLoad]
    public static class DebugSettingsGUI
    {
        private static readonly string[] PlayerToolbarEntries = { "1","2","3","4" };

        static DebugSettingsGUI()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        }
        
        private static void OnToolbarGUI()
        {
            DrawDebugToggle();
            GUILayout.Space(10);
            DrawPlayersToolbar();
        }

        private static void DrawDebugToggle() =>
            DebugSettings.DebugMode = GUILayout.Toggle(DebugSettings.DebugMode, new GUIContent("Debug"), GUILayout.ExpandWidth(false));

        private static void DrawPlayersToolbar()
        {
            DebugSettings.PlayersToWait = GUILayout.Toolbar(DebugSettings.PlayersToWait - 1, PlayerToolbarEntries, GUILayout.ExpandWidth(false)) + 1;
            //GUILayout.Label(new GUIContent("Players", "Players to wait"), GUILayout.ExpandWidth(false));
        }
    }
}
#endif
