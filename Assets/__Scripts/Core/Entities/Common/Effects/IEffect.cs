using System;
using System.Collections.Generic;
using Arena.__Scripts.Core.Entities.Common.Enums;
using UnityEngine;

namespace Arena.__Scripts.Core.Effects
{
    public interface IEffect : IDisposable
    {
        Effect SetTimer(float duration, float tickDuration);
        Effect SetCollection(Dictionary<Type, IEffect> activeEffects);
        Effect SetTarget(GameObject gameObject);
        EffectType GetEffectType();
        void OnApply();
        void OnTick();
        void OnComplete();
    }
}
