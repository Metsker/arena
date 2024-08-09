#if UNITY_EDITOR
using Assemblies.Utilities.Debuging;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Assemblies.Utilities.SceneManagement
{
    [InitializeOnLoad]
    public static class SceneRedirect
    {
        static SceneRedirect()
        {
            UpdatePlayModeStartScene();
        }

        public static void UpdatePlayModeStartScene()
        {
            if (DebugSettings.RedirectScene)
            {
                string scenePath = EditorBuildSettings.scenes[0].path;
                SceneAsset startScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                
                if (startScene != null)
                    EditorSceneManager.playModeStartScene = startScene;
                else
                    Debug.Log("Could not find Scene " + scenePath);
            }
            else
                EditorSceneManager.playModeStartScene = null;
        }
    }
}
#endif