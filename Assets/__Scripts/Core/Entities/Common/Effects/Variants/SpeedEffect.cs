using Arena.__Scripts.Core.Entities.Classes.Common.Stats.DataContainers;
using Arena.__Scripts.Core.Entities.Common.Enums;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Common.Effects.Variants
{
    public class SpeedEffect : Effect
    {
        private float _speedAmount;
        private float _initialSpeed;
        private INetworkDataContainer _classNetworkDataContainer;

        private float _step;

        public void Initialize(float speedAmount, INetworkDataContainer classNetworkDataContainer)
        {
            _classNetworkDataContainer = classNetworkDataContainer;
            _speedAmount = speedAmount;
            _initialSpeed = _classNetworkDataContainer.Speed;
            
            _step = _initialSpeed / ((int)(Duration / TickDuration));;
        }

        public override void OnApply() =>
            _classNetworkDataContainer.AddSpeed(_speedAmount);

        public override void OnTick() =>
            _classNetworkDataContainer.AddSpeed(-_step);

        public override void OnComplete() =>
            _classNetworkDataContainer.SetSpeed(_initialSpeed);

        public override EffectType GetEffectType() =>
            _speedAmount > 0 ? EffectType.Buff : EffectType.Debuff;

        public override int CompareTo(IEffect other)
        {
            if (other is SpeedEffect otherSpeedEffect)
                return Mathf.Abs(_speedAmount).CompareTo(Mathf.Abs(otherSpeedEffect._speedAmount));
            
            return base.CompareTo(other);
        }
    }
}
