using Arena.__Scripts.Core.Entities.Common.Enums;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Sirenix.OdinInspector;

namespace Arena.__Scripts.Core.Entities.Common.Effects.Variants
{
    public class BleedDebuff : Effect, IStackable
    {
        private IHealth _targetHealth;
        private int _tickDamage;
        
        [ShowInInspector]
        public int Stacks { get; set; }
        
        public void Initialize(int tickDamage, IHealth targetHealth)
        {
            _tickDamage = tickDamage;
            _targetHealth = targetHealth;
            
            ResetStacks();
        }

        public override void OnTick()
        {
            if (Stacks == 0)
            {
                Dispose();
                return;
            }
            
            _targetHealth.DealDamageRpc(_tickDamage * Stacks);
        }

        public void ResetStacks() =>
            Stacks = 1;

        public override EffectType GetEffectType() =>
            EffectType.Debuff;
    }
}
