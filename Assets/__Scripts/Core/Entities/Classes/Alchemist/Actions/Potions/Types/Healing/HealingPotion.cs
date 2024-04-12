using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions.Types;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Actions.Potions.Types.Healing
{
    public class HealingPotion : Potion
    {
        private PotionBeltStats.Healing HealingStats => AlchemistNetworkData.PotionBeltStats.healing;

        protected override void OnTrigger(Collider2D col2D)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;
            
            if (col2D.TryGetComponent(out IHealth health))
                health.HealRpc(HealingStats.onHitHealAmount);
        }
    }
}
