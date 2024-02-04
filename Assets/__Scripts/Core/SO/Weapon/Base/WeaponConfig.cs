using UnityEngine;

// ReSharper disable UnusedAutoPropertyAccessor.Local
namespace Arena.__Scripts.Core.SO.Weapon.Base
{
    public abstract class WeaponConfig : ScriptableObject
    {
        [field: SerializeField] [field: Range(0.1f, 5)]
        public float FloatingRange { get; private set; }
    }
}
