using Sirenix.OdinInspector;
using Tower.Core.Entities.Common.Components;
using Unity.Netcode;
using UnityEngine;

namespace Tower.Core.Entities.Classes.Common.Stats.DataContainers
{
    public class HealthNetworkContainer : BaseNetworkHealth
    {
        [SerializeField] private bool autoInit = true;
        
        [Min(0)] [ShowIf(nameof(autoInit))] 
        [SerializeField] private int initialHealth;
        
        [Min(0)] [ShowIf(nameof(autoInit))] 
        [SerializeField] private int initialMaxHealth;
        
        [HideInInspector] public NetworkVariable<int> currentHealth = new ();
        [HideInInspector] public NetworkVariable<int> maxHealth = new ();
        
        public override void OnNetworkSpawn()
        {
            if (!IsServer)
                return;

            if (!autoInit)
                return;

            SetMaxHealthRpc(initialMaxHealth);
            SetCurrentHealthRpc(initialHealth);
        }

        public void ManualInit(int newHealth, int newMaxHealth)
        {
            if (autoInit)
            {
                Debug.LogError("Cannot initialize health manually if autoInit is true.");
                return;
            }

            SetMaxHealthRpc(newMaxHealth);
            SetCurrentHealthRpc(newHealth);
        }
        
        protected override int GetCurrentHealth() =>
            currentHealth.Value;

        [Rpc(SendTo.Server)]
        protected override void SetCurrentHealthRpc(int value) =>
            currentHealth.Value = value;

        protected override int GetMaxHealth() =>
            maxHealth.Value;

        [Rpc(SendTo.Server)]
        protected override void SetMaxHealthRpc(int value) =>
            maxHealth.Value = value;
    }
}
