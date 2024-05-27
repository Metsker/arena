using System;
using System.Collections.Generic;
using System.Linq;
using __Scripts.Assemblies.Network.Serialization;
using Arena.__Scripts.Core.Entities.Common.Effects.Variants;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;

namespace Arena.__Scripts.Core.Entities.Common.Effects
{
    public class EffectsHandler : SerializedNetworkBehaviour
    {
        [OdinSerialize] private readonly Dictionary<Type, IEffect> _activeEffects = new ();
        
        private readonly Dictionary<Type, IEffect> _effectPool = new ();
        public event Action<IEffect> EffectAdded;
        public event Action<IEffect, int> StackableEffectAdded;

        public void TryAddEffect<T>(float duration, float tickDuration = 0) where T : class, IEffect =>
            TryAddEffect<T>(duration, out _, tickDuration);

        [Pure]
        public bool TryAddEffect<T>(float duration, out T effect, float tickDuration = 0) where T : class, IEffect
        {
            if (!_effectPool.TryGetValue(typeof(T), out IEffect element))
            {
                effect = Activator.CreateInstance<T>();
                effect.SetHandler(this);
                
                _effectPool.Add(typeof(T), effect);
            }
            else
            {
                effect = (T)element;
                
                if (_activeEffects.TryGetValue(typeof(T), out IEffect activeEffect))
                {
                    if (activeEffect is IStackable stackable)
                    {
                        stackable.Stacks++;
                        activeEffect.ResetTimer();
                        StackableEffectAdded?.Invoke(activeEffect, stackable.Stacks);
                        return false;
                    }

                    if (effect.CompareTo(activeEffect) <= 0)
                        return false;
                }
                
                effect.Dispose();
            }
            
            _activeEffects.Add(typeof(T), effect);

            if (tickDuration == 0)
                tickDuration = duration;
            
            effect.SetTimer(duration, tickDuration);
            
            EffectAdded?.Invoke(effect);
            
            return true;
        }

        [Button]
        public void ApplyStun()
        {
            TryAddEffect<StunDebuff>(5);
        }
        
        public bool HasEffectOfType<T>() where T : class, IEffect =>
            _activeEffects.ContainsKey(typeof(T));

        public bool TryGetEffectOfType<T>(out T effectOfType) where T : class, IEffect
        {
            effectOfType = default(T);
            
            if (!_activeEffects.TryGetValue(typeof(T), out IEffect effect))
                return false;

            effectOfType = (T)effect;
            return true;
        }
        
        public void RemoveEffectOfType<T>() where T : class, IEffect
        {
            if (!_activeEffects.ContainsKey(typeof(T)))
                return;
            
            _activeEffects[typeof(T)].Dispose();
        }

        public void RemoveEffectsOfType<T>() where T : IEffect =>
            _activeEffects
                .Where(e => e.Key == typeof(T))
                .ForEach(e => e.Value.Dispose());

        public void RemoveEffectOfType(Type effectType) =>
            _activeEffects.Remove(effectType);

        public void RemoveAllEffects() =>
            _activeEffects.ForEach(e => e.Value.Dispose());
    }
}
