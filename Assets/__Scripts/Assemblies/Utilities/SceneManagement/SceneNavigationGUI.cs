#if UNITY_EDITOR
using System.IO;
using Assemblies.Utilities.Debuging;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

namespace Assemblies.Utilities.SceneManagement
{
    [InitializeOnLoad]
    public static class SceneNavigationGUI
    {
        static SceneNavigationGUI()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
        }

        private static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                string name = Path.GetFileNameWithoutExtension(scene.path);

                if (GUILayout.Button(new GUIContent(name, $"Open {name} Scene")))
                    EditorSceneManager.OpenScene(scene.path);
            }
            GUILayout.Space(10);
            DebugSettings.RedirectScene = GUILayout.Toggle(DebugSettings.RedirectScene, "Redirect");
            SceneRedirect.UpdatePlayModeStartScene();
        }
    }
}
#endif