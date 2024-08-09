using UnityEngine;

namespace Assemblies.Utilities.Extensions
{
    public static class MonoBehaviorExtensions
    {
        public static void Activate(this MonoBehaviour mono) =>
            mono.gameObject.SetActive(true);
        
        public static void Deactivate(this MonoBehaviour mono) =>
            mono.gameObject.SetActive(false);
    }
}
