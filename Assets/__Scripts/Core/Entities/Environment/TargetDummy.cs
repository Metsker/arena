using __Scripts.Assemblies.Utilities.Timer;
using Arena.__Scripts.Core.Entities.Classes.Common.Stats.DataContainers;
using Unity.Netcode;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Environment
{
    public class TargetDummy : NetworkBehaviour
    {
        [SerializeField] private ParticleSystem fxParticleSystem;
        
        [SerializeField] private float resetSec;
        [Header("Data")]
        [SerializeField] private HealthNetworkContainer healthNetworkContainer;

        private CountdownTimer _countdownTimer;

        private void Awake()
        {
            if (NetworkManager.IsServer)
            {
                _countdownTimer = new CountdownTimer(resetSec)
                {
                    OnTimerStop = healthNetworkContainer.FullHealRpc
                };
            }
        }

        public override void OnNetworkSpawn() =>
            healthNetworkContainer.currentHealth.OnValueChanged += OnHealthChanged;

        public override void OnNetworkDespawn() =>
            healthNetworkContainer.currentHealth.OnValueChanged -= OnHealthChanged;

        private void Update()
        {
            if (IsServer)
                _countdownTimer.Tick(Time.deltaTime);
        }

        private void OnHealthChanged(int previousvalue, int newvalue)
        {
            if (newvalue < previousvalue)
                fxParticleSystem.Play();

            if (IsServer)
                _countdownTimer.Start();
        }
    }
}
