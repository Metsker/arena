using Assemblies.Network.Serialization;
using Assemblies.Utilities.Timers;
using Sirenix.Serialization;
using Tower.Core.Entities.Common.Interfaces;
using UnityEngine;

namespace Tower.Core.Entities.Environment
{
    public class TargetDummy : SerializedNetworkBehaviour
    {
        [SerializeField] private ParticleSystem fxParticleSystem;
        
        [SerializeField] private float resetSec;
        [Header("Data")]
        [OdinSerialize] private IHealth health;

        private CountdownTimer _countdownTimer;

        private void Awake()
        {
            if (NetworkManager.IsServer)
            {
                _countdownTimer = new CountdownTimer(resetSec)
                {
                    OnTimerStop = health.FullHealRpc
                };
            }
        }

        public override void OnNetworkSpawn() =>
            health.HealthChanged += OnHealthChanged;

        public override void OnNetworkDespawn() =>
            health.HealthChanged -= OnHealthChanged;

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
