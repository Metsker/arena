using UnityEditor;

namespace Assemblies.Utilities.Debuging
{
    public static class DebugSettings
    {
        public static bool DebugMode {
            get => EditorPrefs.GetBool("Debug", false);
            set => EditorPrefs.SetBool("Debug", value);
        }
        
        public static bool RedirectScene {
            get => EditorPrefs.GetBool("RedirectScene", true);
            set => EditorPrefs.SetBool("RedirectScene", value);
        }
        
        public static int PlayersToWait 
        {
            get => EditorPrefs.GetInt("PlayersToWait", 1);
            set => EditorPrefs.SetInt("PlayersToWait", value);
        }
    }
}
