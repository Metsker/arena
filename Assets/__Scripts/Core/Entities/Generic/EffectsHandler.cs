using System.Collections.Generic;
using Arena.__Scripts.Generic.Effects;
using UnityEngine;
namespace Arena.__Scripts.Core.Entities.Generic
{
    public class EffectsHandler : MonoBehaviour
    {
        private readonly List<IEffect> _effects = new ();
        
        public void Update()
        {
            if (_effects.Count <= 0)
                return;
            
            foreach (IEffect effect in _effects)
            {
                effect.Timer.Tick(Time.deltaTime);
                effect.OnTick(effect.Timer.GetTime());
            }
        }

        public void AddEffect(IEffect effect)
        {
            _effects.Add(effect);

            effect.Timer.OnTimerStart = effect.OnApply;
            effect.Timer.OnTimerStop = () =>
            {
                effect.OnTimeOut();
                _effects.Remove(effect);
            };
            effect.Timer.Start();
        }
        
        public void RemoveAllEffectsOfType<T>() where T : IEffect
        {
            int length = _effects.Count;
            for (int i = length - 1; i >= 0; i--)
            {
                IEffect effect = _effects[i];
                
                if (_effects[i] is not T)
                    continue;
                
                _effects.Remove(effect);
            }
        }
    }
}
