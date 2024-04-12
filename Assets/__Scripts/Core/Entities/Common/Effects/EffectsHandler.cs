using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Arena.__Scripts.Core.Effects;
using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Arena.__Scripts.Core.Entities.Common.Effects.Variants;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine.InputSystem;

namespace Arena.__Scripts.Core.Entities.Common.Effects
{
    public class EffectsHandler : SerializedMonoBehaviour
    {
        [OdinSerialize] private readonly Dictionary<Type, IEffect> _activeEffects = new ();
        private readonly Dictionary<Type, IEffect> _effectPool = new ();

        public event Action EffectsChanged;

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public T AddEffect<T>(float duration, float tickDuration = 0) where T : class, IEffect
        {
            T effect;
            if (!_effectPool.TryGetValue(typeof(T), out IEffect element))
            {
                effect = Activator.CreateInstance<T>();
                effect
                    .SetCollection(_activeEffects)
                    .SetTarget(gameObject);
                
                _effectPool.Add(typeof(T), effect);
            }
            else
            {
                effect = element as T;
                effect.Dispose();
            }
            _activeEffects.Add(typeof(T), effect);

            if (tickDuration == 0)
                tickDuration = duration;
            
            effect.SetTimer(duration, tickDuration);
            
            EffectsChanged?.Invoke();
            
            return effect;
        }

        [Button]
        public void ApplyStun() =>
            AddEffect<StunDebuff>(5).Initialize(GetComponent<ActionToggler>());

        public bool HasEffectOfType<T>() where T : class, IEffect =>
            _activeEffects.ContainsKey(typeof(T));

        public void RemoveEffectOfType<T>() where T : class, IEffect
        {
            if (!_activeEffects.ContainsKey(typeof(T)))
                return;
            
            _activeEffects[typeof(T)].Dispose();
            _activeEffects.Remove(typeof(T));
            
            EffectsChanged?.Invoke();
        }

        public void RemoveEffectsOfType<T>() where T : IEffect
        {
            _activeEffects
                .Where(e => e.Key == typeof(T))
                .ForEach(e => e.Value.Dispose());
            
            _activeEffects.Clear();
            
            EffectsChanged?.Invoke();
        }

        public void RemoveAllEffects()
        {
            _activeEffects.ForEach(e => e.Value.Dispose());
            _activeEffects.Clear();
            
            EffectsChanged?.Invoke();
        }
    }
}
