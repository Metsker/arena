#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace __Scripts.Assemblies.Utilities.SceneManagement
{
    [InitializeOnLoad]
    public static class SceneEnumGenerator
    {
        private const string EnumName = "ScenesInBuild";

        static SceneEnumGenerator()
        {
            EditorBuildSettings.sceneListChanged += EditorBuildSettingsOnSceneListChanged;
        }

        private static void EditorBuildSettingsOnSceneListChanged() =>
            EnumGenerator.Generate(EnumName, InBuildSceneNames().ToArray());

        private static IEnumerable<string> InBuildSceneNames()
        {
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (!scene.enabled)
                    continue;
                
                yield return Path.GetFileNameWithoutExtension(scene.path);
            }
        }
    }
}
#endif