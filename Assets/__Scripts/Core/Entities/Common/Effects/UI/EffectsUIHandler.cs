using System.Collections.Generic;
using NTC.Pool;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Tower.Core.Entities.Common.Effects.UI
{
    public class EffectsUIHandler : MonoBehaviour
    {
        [SerializeField] private EffectView effectUIElementPrefab;
        [SerializeField] private RectTransform root;

        private readonly Dictionary<string, EffectView> _activeViews = new ();
        private readonly Dictionary<string, Sprite> _activeAssets = new ();

        public async void AddOrUpdate(string key, float duration, int stacks = 0)
        {
            if (_activeViews.TryGetValue(key, out EffectView view))
                view.Timer.Reset(duration);
            else
            {
                float timeBeforeLoad = Time.time;
                Sprite sprite = await Addressables.LoadAssetAsync<Sprite>(key).Task;

                view = NightPool.Spawn(effectUIElementPrefab, root);
                view.SetSprite(sprite);
                view.Timer.OnTimerStop = () => Remove(key);
                
                view.Timer.Start(duration - (Time.time - timeBeforeLoad));

                _activeViews.Add(key, view);
                _activeAssets.Add(key, sprite);
            }

            if (stacks > 0)
                view.SetStacks(stacks);
            else
                view.RemoveStacks();
        }
        
        public void Remove(string key)
        {
            EffectView view = _activeViews[key];
            view.Timer.Stop();
            NightPool.Despawn(view);
            //Addressables.Release(_activeAssets[key]);
            
            _activeViews.Remove(key);
            _activeAssets.Remove(key);
        }
    }
}
