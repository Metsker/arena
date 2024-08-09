using Tower.Core.Entities.Common.Effects.Variants.Base;
using Tower.Core.Entities.Common.Enums;
using Tower.Core.Entities.Common.Interfaces;
using UnityEngine.AddressableAssets;

namespace Tower.Core.Entities.Common.Effects.Variants.Debuffs
{
    public class BleedDebuff : Effect, IStackable
    {
        public int Stacks { get; set; }
        
        private readonly IHealth _targetHealth;
        private readonly int _tickDamage;

        public BleedDebuff(
            IKeyEvaluator key,
            float duration,
            float tickDuration,
            int tickDamage,
            IHealth targetHealth) : base(key, duration, tickDuration)
        {
            _tickDamage = tickDamage;
            _targetHealth = targetHealth;
        }

        public override void OnTick()
        {
            base.OnTick();
            
            _targetHealth.DealDamageRpc(_tickDamage * Stacks);
        }

        public override EffectSide GetEffectSide() =>
            EffectSide.Debuff;
    }
}
