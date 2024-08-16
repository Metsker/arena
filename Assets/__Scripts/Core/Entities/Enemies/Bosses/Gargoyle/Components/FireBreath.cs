using NTC.Pool;
using Tower.Core.Entities.Environment;
using Unity.Netcode;
using UnityEngine;

namespace Tower.Core.Entities.Enemies.Bosses.Gargoyle.Components
{
    public class FireBreath : NetworkBehaviour
    {
        [SerializeField] private Vfx fireVfxPrefab;
        [SerializeField] private Transform fireOrigin;
        public bool IsPlaying => _fireFx != null && _fireFx.IsPlaying;
        
        private Vfx _fireFx;

        [Rpc(SendTo.Everyone)]
        public void PerformFireBreathRpc(Vector3 lookAt)
        {
            _fireFx = NightPool.Spawn(fireVfxPrefab, fireOrigin);
            _fireFx.transform.LookAt(lookAt);
        }
    }
}
