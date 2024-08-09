using System;
using Tower.Core.Entities.Common.Enums;

namespace Tower.Core.Entities.Common.Effects.Variants.Base
{
    public interface IEffect : IDisposable
    {
        float Duration { get; }
        string Key { get; }
        IEffect SetHandler(EffectsHandler handler);
        EffectSide GetEffectSide();
        void OnApply();
        void OnTick();
        void ResetTimer(float newDuration);
        void OnDispose();
        void StartTimer();
    }
}
