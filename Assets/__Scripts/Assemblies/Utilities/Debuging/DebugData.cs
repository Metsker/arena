using UnityEngine;

namespace __Scripts.Assemblies.Utilities.Debuging
{
    [CreateAssetMenu(fileName = "DebugData", menuName = "Debug", order = 0)]
    public class DebugData : ScriptableObject
    {
        public static bool IsDebug { get; private set; }

        public bool debugField;

        private void OnEnable() =>
            IsDebug = debugField;
    }
}
