using UnityEngine;

namespace __Scripts.Assemblies.Utilities.Debuging
{
    [CreateAssetMenu(fileName = "DebugData", menuName = "Debug", order = 0)]
    public class DebugData : ScriptableObject
    {
        public static bool IsDebug { get; private set; }
        public static int PlayersToWait { get; private set; }

        public bool isDebug;
        public int playersToWait = 2;

        private void OnEnable()
        {
            IsDebug = isDebug;
            PlayersToWait = playersToWait;
        }
    }
}
