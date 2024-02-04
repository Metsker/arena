using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using Arena.__Scripts.Core.Entities.Generic;
using Arena.__Scripts.General.Effects;
using UnityEngine;
namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions.Toxin
{
    public class ToxinDebuff : Debuff
    {
        private readonly float _interval;
        private readonly float _damagePercent;
        private readonly IHealth _targetHealth;

        public ToxinDebuff(PotionBeltData.Toxin toxinStats, IHealth targetHealth) : base(toxinStats.duration)
        {
            _damagePercent = toxinStats.percentPerInterval;
            _interval = toxinStats.intervalSec;
            _targetHealth = targetHealth;
        }

        public override void OnTick(float remainingTime)
        {
            if (remainingTime % _interval == 0)
            {
                Debug.Log("CHECK");
                
                _targetHealth.DealDamage(_damagePercent);
            }
        }
    }
}
