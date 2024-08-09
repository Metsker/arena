using Sirenix.OdinInspector;
using Tower.Core.Entities.Classes.Common.Components.InputActions;
using UnityEngine;
using VContainer;

namespace Tower.Core.Entities.Classes.Common.Components.UI
{
    public class UltimateVisualizer : MonoBehaviour
    {
        private PlayerGlobalCanvas _playerGlobalCanvas;
        private IClassUltimate _classUltimate;

        [Inject]
        private void Construct(IClassUltimate classUltimate, PlayerGlobalCanvas playerGlobalCanvas)
        {
            _classUltimate = classUltimate;
            _playerGlobalCanvas = playerGlobalCanvas;
        }

        private void OnEnable() =>
            _classUltimate.UltMeter.OnValueChanged += OnStacksChanged;

        private void OnDisable() =>
            _classUltimate.UltMeter.OnValueChanged -= OnStacksChanged;

        private void OnStacksChanged(int _, int newValue)
        {
            float fillAmount = (float)newValue / _classUltimate.MaxStacks;
            
            if (newValue == 0)
                _playerGlobalCanvas.UltProgressBar.Fill(fillAmount, _classUltimate.Duration);
            else
                _playerGlobalCanvas.UltProgressBar.Fill(fillAmount);
        }
    }
}
