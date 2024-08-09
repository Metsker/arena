using Tower.Core.Entities.Classes.Common.Components.InputActions;
using Tower.Core.Entities.Common.Effects.Variants.Base;
using Tower.Core.Entities.Common.Enums;
using Tower.Core.Entities.Common.Interfaces.Toggleables;
using UnityEngine.AddressableAssets;

namespace Tower.Core.Entities.Common.Effects.Variants.Debuffs
{
    public class StunDebuff : Effect
    {
        private ActionToggler _actionToggler;

        public StunDebuff(IKeyEvaluator key, float duration) : base(key, duration)
        {
        }

        public override void OnApply()
        {
            _actionToggler = Target.GetComponent<ActionToggler>();
            
            _actionToggler.DisableAll(ChargableDisableMode.Release, true);
            _actionToggler.Enable<IToggleablePhysics>();
        }

        public override void OnDispose()
        {
            if (Handler.Any<StunDebuff>())
                return;
            
            _actionToggler.EnableAll();
        }

        public override EffectSide GetEffectSide() =>
            EffectSide.Debuff;
    }
}
