using System;
using System.Collections;
using System.Collections.Generic;
using Arena.__Scripts.Core.Entities.Common.Enums;
using UniRx;
using UnityEngine;

namespace Arena.__Scripts.Core.Effects
{
    public abstract class Effect : IEffect
    {
        protected GameObject Target;
        protected int TickCount;
        
        private IDisposable _timerDisposable;
        private Dictionary<Type, IEffect> _activeEffects;

        public Effect SetTimer(float duration, float tickDuration)
        {
            TickCount = (int)(duration / tickDuration);
            
            _timerDisposable = Observable
                .FromCoroutine(() => Timer(duration, tickDuration))
                .Subscribe();
            
            return this;
        }

        public Effect SetCollection(Dictionary<Type, IEffect> activeEffects)
        {
            _activeEffects = activeEffects;
            return this;
        }

        public Effect SetTarget(GameObject gameObject)
        {
            Target = gameObject;
            return this;
        }

        public virtual void OnApply(){}

        public virtual void OnTick(){}

        public virtual void OnComplete(){}

        public void Dispose()
        {
            OnComplete();
            
            _timerDisposable?.Dispose();
            _activeEffects.Remove(GetType());
        }

        private IEnumerator Timer(float duration, float tickDuration)
        {
            OnApply();
            while (duration >= tickDuration)
            {
                yield return new WaitForSeconds(tickDuration);

                duration -= tickDuration;

                OnTick();
            }
            Dispose();
        }

        public abstract EffectType GetEffectType();
    }
}
