﻿using Arena.__Scripts.Core.Effects;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using Arena.__Scripts.Core.Entities.Common;
using Arena.__Scripts.Core.Entities.Common.Enums;
using Arena.__Scripts.Core.Entities.Common.Interfaces;

namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions.Types.Toxin
{
    public class DamageOverTimeDebuff : Effect
    {
        private IHealth _targetHealth;
        private float _percentPerTick;
        private int _onHitDamage;

        public void Initialize(float percentPerTick, int onHitDamage, IHealth targetHealth)
        {
            _percentPerTick = percentPerTick;
            _targetHealth = targetHealth;
            _onHitDamage = onHitDamage;
        }

        public override void OnApply() =>
            _targetHealth.DealDamageRpc(_onHitDamage);

        public override void OnTick() =>
            _targetHealth.DealDamageRpc(_percentPerTick);

        public override EffectType GetEffectType() =>
            EffectType.Debuff;
    }
}
