#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace __Scripts.Assemblies.Utilities.SceneManagement
{
    [InitializeOnLoad]
    public static class SceneRedirect
    {
        static SceneRedirect()
        {
            SetPlayModeStartScene(EditorBuildSettings.scenes[0].path);
        }

        private static void SetPlayModeStartScene(string scenePath)
        {
            SceneAsset startScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            
            if (startScene != null)
                EditorSceneManager.playModeStartScene = startScene;
            else
                Debug.Log("Could not find Scene " + scenePath);
        }
    }
}
#endif