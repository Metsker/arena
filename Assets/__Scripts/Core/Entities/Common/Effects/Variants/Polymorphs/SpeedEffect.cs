using Tower.Core.Entities.Classes.Common.Stats.DataContainers;
using Tower.Core.Entities.Common.Effects.Variants.Base;
using Tower.Core.Entities.Common.Enums;
using UnityEngine.AddressableAssets;

namespace Tower.Core.Entities.Common.Effects.Variants.Polymorphs
{
    public class SpeedEffect : Effect
    {
        private readonly float _speedAmount;
        private readonly INetworkDataContainer _classNetworkDataContainer;

        protected SpeedEffect(
            IKeyEvaluator key,
            float duration,
            float tickDuration,
            float speedAmount,
            INetworkDataContainer classNetworkDataContainer) : base(key, duration, tickDuration)
        {
            _speedAmount = speedAmount;
            _classNetworkDataContainer = classNetworkDataContainer;
        }

        public override void OnApply() =>
            _classNetworkDataContainer.AddSpeedRpc(_speedAmount);

        public override void OnDispose() =>
            _classNetworkDataContainer.AddSpeedRpc(-_speedAmount);

        public override EffectSide GetEffectSide() =>
            _speedAmount > 0 ? EffectSide.Buff : EffectSide.Debuff;
    }
}
