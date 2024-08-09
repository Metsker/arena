using Tower.Core.Entities.Classes.Alchemist.Data;
using Tower.Core.Entities.Common.Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace Tower.Core.Entities.Classes.Alchemist.Actions.Potions.Types.Healing
{
    public class HealingPotion : Potion
    {
        private PotionsStats.Healing HealingStats => AlchemistData.PotionsStats.healing;

        protected override void OnTrigger(Collider2D col2D)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;

            if (!col2D.transform.root.TryGetComponent(out IHealth health))
                return;
            
            health.HealRpc(HealingStats.onHitHealAmount + (int)(AlchemistData.MaxHealth * HealingStats.maxHPScaling));
            StackUltimate(HealingStats.overheat);
        }
    }
}
