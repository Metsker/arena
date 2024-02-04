using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using Arena.__Scripts.Core.Entities.Generic;
using Arena.__Scripts.Core.Network;
using UnityEngine;
namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions.Healing
{
    public class HealingPotion : PotionBase
    {
        private PotionBeltData.Healing HealingStats => PotionBeltData.healing;

        protected override void OnCollision(Collision2D collision)
        {
            base.OnCollision(collision);
            
            if (collision.otherRigidbody.TryGetComponent(out IHealth health))
                health.Heal(HealingStats.onHitHealAmount);
        }
    }
}
