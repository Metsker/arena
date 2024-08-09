using NTC.Pool;
using Tower.Core.Entities.Environment;
using UnityEngine;

namespace Tower.Core.Entities.Enemies.Bosses.Gargoyle.Components
{
    public class FireBreath : MonoBehaviour
    {
        [SerializeField] private Vfx fireVfxPrefab;
        [SerializeField] private Transform fireOrigin;
        public bool IsPlaying => _fireFx != null && _fireFx.IsPlaying;
        
        private Vfx _fireFx;

        public void PerformFireBreath(Vector3 position)
        {
            _fireFx = NightPool.Spawn(fireVfxPrefab, fireOrigin);
            _fireFx.transform.LookAt(position);
        }
    }
}
