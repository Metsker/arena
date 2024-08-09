using System.Collections.Generic;
using System.Linq;
using Assemblies.Network.Serialization;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Tower.Core.Entities.Classes.Common.Stats.DataContainers;
using Tower.Core.Entities.Common.Effects.UI;
using Tower.Core.Entities.Common.Effects.Variants.Base;
using Tower.Core.Entities.Common.Effects.Variants.Debuffs;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Tower.Core.Entities.Common.Effects
{
    public class EffectsHandler : SerializedNetworkBehaviour
    {
        [SerializeField] private EffectsUIHandler uiHandler;
        
        [OdinSerialize] private Dictionary<string, IEffect> _activeEffects = new ();
        
        public void Add(IEffect effect)
        {
            if (_activeEffects.TryGetValue(effect.Key, out IEffect activeEffect))
            {
                activeEffect.ResetTimer(effect.Duration);
                effect = activeEffect;
            }
            else
            {
                effect.SetHandler(this)
                    .StartTimer();
                
                _activeEffects.Add(effect.Key, effect);
            }
            
            if (effect is IStackable stackable)
            {
                stackable.Stacks++;
                AddOrUpdateStackableUIRpc(effect.Key, effect.Duration, stackable.Stacks);
            }
            else
                AddOrUpdateUIRpc(effect.Key, effect.Duration);
        }

        [Button]
        public void ApplyStun(AssetReference reference, int time)
        {
            uiHandler = FindFirstObjectByType<EffectsUIHandler>();
            Add(new BleedDebuff(reference, time, 2, 20, FindFirstObjectByType<HealthNetworkContainer>()));
        }
        
        public bool TryGet<T>(AssetReference reference, out T effect) where T : class, IEffect
        {
            if (_activeEffects.TryGetValue(reference.RuntimeKey.ToString(), out IEffect activeEffect))
            {
                effect = (T)activeEffect;
                return true;
            }
            effect = default(T);
            return false;
        }

        public bool Any<T>() =>
            _activeEffects.Any(x => x.Value is T);

        public void Remove(string key, bool inform = false)
        {
            _activeEffects.Remove(key);

            if (inform)
                RemoveUIRpc(key);
        }

        [Rpc(SendTo.Everyone)]
        private void AddOrUpdateUIRpc(string key, float duration) =>
            uiHandler?.AddOrUpdate(key, duration);
        
        [Rpc(SendTo.Everyone)]
        private void AddOrUpdateStackableUIRpc(string key, float duration, int stacks) =>
            uiHandler?.AddOrUpdate(key, duration, stacks);

        [Rpc(SendTo.Everyone)]
        private void RemoveUIRpc(string key) =>
            uiHandler?.Remove(key);
    }
}
