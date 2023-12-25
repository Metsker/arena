using UnityEngine;

// ReSharper disable UnusedAutoPropertyAccessor.Local
namespace Arena._Project._Scripts.ScriptableObjects.Weapon.Base
{
    public abstract class WeaponConfig : ScriptableObject
    {
        [field: SerializeField] [field: Range(0.1f, 5)]
        public float FloatingRange { get; private set; }
    }
}
