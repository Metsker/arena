using Arena.__Scripts.Core.Effects;
using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Arena.__Scripts.Core.Entities.Common.Enums;

namespace Arena.__Scripts.Core.Entities.Common.Effects.Variants
{
    public class StunDebuff : Effect
    {
        private ActionToggler _actionToggler;

        public void Initialize(ActionToggler actionToggler)
        {
            _actionToggler = actionToggler;
        }

        public override void OnApply()
        {
            _actionToggler.DisableAll(ChargableDisableMode.Release, true);
        }

        public override void OnComplete()
        {
            _actionToggler.EnableAll();
        }

        public override EffectType GetEffectType() =>
            EffectType.Debuff;
    }
}
