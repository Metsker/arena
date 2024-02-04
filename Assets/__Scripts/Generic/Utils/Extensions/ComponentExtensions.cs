using UnityEngine;
namespace Arena.__Scripts.Generic.Utils.Extensions
{
    public static class ComponentExtensions
    {
        public static bool HasComponent<T>(this Component obj) =>
            obj.GetComponent<T>() != null;
    }
}
