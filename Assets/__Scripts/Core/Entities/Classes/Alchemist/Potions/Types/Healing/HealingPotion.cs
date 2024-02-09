using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using Arena.__Scripts.Core.Entities.Generic;
using UnityEngine;
namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions.Types.Healing
{
    public class HealingPotion : Potion
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
