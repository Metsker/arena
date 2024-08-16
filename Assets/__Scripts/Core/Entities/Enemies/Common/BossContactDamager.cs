using System;
using Tower.Core.Entities.Classes.Common.Components;
using Tower.Core.Entities.Common.Interfaces;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Tower.Core.Entities.Enemies.Common
{
    public class BossContactDamager : NetworkBehaviour
    {
        private IBossDataContainer _bossDataContainer;

        [Inject]
        private void Construct(IBossDataContainer bossDataContainer)
        {
            _bossDataContainer = bossDataContainer;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsServer)
                return;
            
            if (other.TryGetComponent(out HitboxMediator hitboxMediator))
            {
                hitboxMediator.Health.DealDamageRpc(_bossDataContainer.CurrentStage.damage);
                print(hitboxMediator.Health.CurrentHealth);
                print(_bossDataContainer.CurrentStage.damage);
            }
        }
    }
}
