#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

namespace __Scripts.Assemblies.Utilities.SceneManagement
{
    [InitializeOnLoad]
    public class SceneNavGUI
    {
        static SceneNavGUI()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
        }

        private static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                string path = scene.path;
                string name = Path.GetFileNameWithoutExtension(path);

                if (GUILayout.Button(new GUIContent(name, $"Start {name} Scene")))
                    EditorSceneManager.OpenScene(path);
            }
        }
    }
}
#endif