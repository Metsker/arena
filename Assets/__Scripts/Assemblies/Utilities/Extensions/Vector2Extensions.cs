using UnityEngine;

namespace Assemblies.Utilities.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 With(this Vector2 vector, float? x = null, float? y = null) {
            return new Vector2(x ?? vector.x, y ?? vector.y);
        }

        public static Vector2 Add(this Vector2 vector, float x = 0, float y = 0) {
            return new Vector2(vector.x + x, vector.y + y);
        }
        
        public static Vector3 WithZ(this Vector2 vector, float z) {
            return new Vector3(vector.x, vector.y, z);
        }

        public static bool InRangeOf(this Vector2 current, Vector2 target, float range) {
            return (current - target).sqrMagnitude <= range * range;
        }
    }
}
