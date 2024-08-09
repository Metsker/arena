using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Tower.Core.Entities.Common.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Tower.Core.Entities.Common.UI
{
    public class HealthBar : SerializedMonoBehaviour
    {
        [SerializeField] private bool autoInit;
        [ShowIf(nameof(autoInit))]
        [OdinSerialize] private IHealth health;
        [SerializeField] private Image bar;
        
        private TweenerCore<float, float, FloatOptions> _tween;

        public void SetHealth(IHealth newHealth)
        {
            if (autoInit)
            {
                Debug.LogError("Cannot initialize health manually if autoInit is true.");
                return;
            }
            health = newHealth;
            health.HealthChanged += OnHealthChanged;
        }

        public void OnEnable()
        {
            if (!autoInit)
                return;
            
            health.HealthChanged += OnHealthChanged;
        }

        public void OnDisable() =>
            health.HealthChanged -= OnHealthChanged;

        private void Start() =>
            UpdateUI();

        private void OnHealthChanged(int _, int __) =>
            UpdateUI();

        private void UpdateUI()
        {
            _tween?.Kill();
            _tween = bar.DOFillAmount(health.RemainingHeathNormalized, 0.2f);
        }
    }
}
