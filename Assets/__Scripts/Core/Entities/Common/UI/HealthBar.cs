using System;
using Arena.__Scripts.Core.Entities.Classes.Common.Stats.DataContainers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Arena.__Scripts.Core.Entities.Common.UI
{
    public class HealthBar : NetworkBehaviour
    {
        [SerializeField] private HealthNetworkContainer healthNetworkContainer;
        [SerializeField] private Image bar;

        public override void OnNetworkSpawn() =>
            healthNetworkContainer.currentHealth.OnValueChanged += OnHealthChanged;

        public override void OnNetworkDespawn() =>
            healthNetworkContainer.currentHealth.OnValueChanged -= OnHealthChanged;

        private void Start() =>
            UpdateUI();

        private void OnHealthChanged(int previousvalue, int newvalue) =>
            UpdateUI();

        private void UpdateUI() =>
            bar.fillAmount = healthNetworkContainer.RemainingHeathNormalized;
    }
}
