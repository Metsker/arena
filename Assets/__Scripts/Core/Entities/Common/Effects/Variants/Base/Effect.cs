using System;
using System.Collections;
using Tower.Core.Entities.Common.Enums;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Tower.Core.Entities.Common.Effects.Variants.Base
{
    public abstract class Effect : IEffect
    {
        public string Key { get; }
        public float Duration { get; private set; }
        protected ulong HandlerNetworkObjectId => Handler.NetworkObjectId;
        protected GameObject Target => Handler.gameObject;

        protected EffectsHandler Handler;
        protected readonly float TickDuration;

        private IDisposable _timerDisposable;

        protected Effect(IKeyEvaluator key, float duration, float tickDuration = 0)
        {
            Duration = duration;
            TickDuration = tickDuration == 0 ? duration : tickDuration;

            Key = key.RuntimeKey.ToString();
        }
        
        public IEffect SetHandler(EffectsHandler handler)
        {
            Handler = handler;
            return this;
        }

        public void StartTimer() =>
            _timerDisposable = Observable
                .FromCoroutine(() => Timer(TickDuration))
                .Subscribe();

        public void ResetTimer(float newDuration)
        {
            _timerDisposable.Dispose();
            Duration = newDuration;
            StartTimer();
        }

        public virtual void OnApply(){}

        public virtual void OnTick(){}

        public virtual void OnDispose(){}

        public void Dispose()
        {
            _timerDisposable?.Dispose();
            Handler.Remove(Key);
            
            OnDispose();
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

        public abstract EffectSide GetEffectSide();
    }
}
