using Tower.Core.Entities.Common.BT;
using Tower.Core.Entities.Common.Interfaces;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Tower.Core.Entities.Enemies.Bosses
{
    public class BossHealthListener : NetworkBehaviour
    {
        [SerializeField] private NetworkBonsaiTree tree;
        
        private IHealth _health;

        [Inject]
        private void Construct(IHealth health)
        {
            _health = health;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
                return;

            _health.HealthChanged += OnHealthChanged;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer)
                return;
            
            _health.HealthChanged -= OnHealthChanged;
        }
        
        private void OnHealthChanged(int _, int __)
        {
            if (_health.RemainingHeathNormalized <= 0.5f)
                tree.Tree.blackboard.Set("BelowHalf", true);
            else
                tree.Tree.blackboard.Unset("BelowHalf");
            
            if (_health.Dead)
                tree.Tree.blackboard.Set("Dead", true);
        }
    }
}
