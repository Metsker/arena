using System;
using Arena.__Scripts.Core.Entities.Common.Enums;

namespace Arena.__Scripts.Core.Entities.Common.Effects
{
    public interface IEffect : IDisposable, IComparable<IEffect>
    {
        float Duration { get; }
        Effect SetTimer(float duration, float tickDuration);
        Effect SetHandler(EffectsHandler handler);
        EffectType GetEffectType();
        void OnApply();
        void OnTick();
        void ResetTimer();
        void OnComplete();
    }
}
