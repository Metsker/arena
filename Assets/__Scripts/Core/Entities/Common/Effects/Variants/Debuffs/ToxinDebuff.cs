using Tower.Core.Entities.Common.Effects.Variants.Base;
using Tower.Core.Entities.Common.Enums;
using Tower.Core.Entities.Common.Interfaces;
using UnityEngine.AddressableAssets;

namespace Tower.Core.Entities.Common.Effects.Variants.Debuffs
{
    public class ToxinDebuff : Effect
    {
        private readonly IHealth _targetHealth;
        private readonly float _percentPerTick;

        public ToxinDebuff(
            IKeyEvaluator key,
            float duration,
            float tickDuration,
            float percentPerTick,
            IHealth targetHealth) : base(key, duration, tickDuration)
        {
            _percentPerTick = percentPerTick;
            _targetHealth = targetHealth;
        }

        public override void OnTick() =>
            _targetHealth.DealDamageRpc(_percentPerTick);

        public override EffectSide GetEffectSide() =>
            EffectSide.Debuff;
    }
}
