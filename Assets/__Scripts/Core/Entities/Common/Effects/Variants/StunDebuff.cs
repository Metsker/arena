using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Arena.__Scripts.Core.Entities.Common.Enums;
using Arena.__Scripts.Core.Entities.Common.Interfaces.Toggleables;

namespace Arena.__Scripts.Core.Entities.Common.Effects.Variants
{
    public class StunDebuff : Effect
    {
        private ActionToggler _actionToggler;

        public override void OnApply()
        {
            _actionToggler = Target.GetComponent<ActionToggler>();
            
            _actionToggler.DisableAll(ChargableDisableMode.Release, true);
            _actionToggler.Enable<IToggleablePhysics>();
        }

        public override void OnComplete()
        {
            _actionToggler.EnableAll();
        }

        public override EffectType GetEffectType() =>
            EffectType.Debuff;
    }
}
