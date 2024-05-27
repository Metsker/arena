using System;
using System.Collections;
using Arena.__Scripts.Core.Entities.Common.Enums;
using UniRx;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Common.Effects
{
    public abstract class Effect : IEffect
    {
        public float Duration { get; private set; }
        
        protected float TickDuration;
        protected ulong NetworkObjectId => _handler.NetworkObjectId;
        protected Transform Target => _handler.transform;
        
        private float _startDuration;

        private IDisposable _timerDisposable;
        private EffectsHandler _handler;

        public Effect SetTimer(float duration, float tickDuration)
        {
            Duration = duration;
            TickDuration = tickDuration;
            _startDuration = duration;
            
            _timerDisposable = Observable
                .FromCoroutine(() => Timer(tickDuration))
                .Subscribe();
            
            return this;
        }

        public Effect SetHandler(EffectsHandler handler)
        {
            _handler = handler;
            return this;
        }

        public virtual void OnApply(){}

        public virtual void OnTick(){}

        public virtual void OnComplete(){}

        public void ResetTimer() =>
            Duration = _startDuration;

        public void Dispose()
        {
            OnComplete();
            
            _timerDisposable?.Dispose();
            _handler.RemoveEffectOfType(GetType());
        }

        private IEnumerator Timer(float tickDuration)
        {
            OnApply();
            while (Duration >= tickDuration)
            {
                yield return new WaitForSeconds(tickDuration);

                Duration -= tickDuration;

                OnTick();
            }
            Dispose();
        }

        public abstract EffectType GetEffectType();
        
        public virtual int CompareTo(IEffect other) =>
            Duration.CompareTo(other.Duration);
    }
}
