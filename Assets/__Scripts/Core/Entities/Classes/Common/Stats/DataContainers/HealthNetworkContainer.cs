using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Stats.DataContainers
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

            SetMaxHealth(initialMaxHealth);
            SetCurrentHealth(initialHealth);
        }

        public void ManualInit(int newHealth, int newMaxHealth)
        {
            if (autoInit)
            {
                Debug.LogError("Cannot initialize health manually if autoInit is true.");
                return;
            }

            SetMaxHealth(newMaxHealth);
            SetCurrentHealth(newHealth);
        }
        
        protected override int GetCurrentHealth() =>
            currentHealth.Value;

        protected override void SetCurrentHealth(int value) =>
            currentHealth.Value = value;

        protected override int GetMaxHealth() =>
            maxHealth.Value;

        protected override void SetMaxHealth(int value) =>
            maxHealth.Value = value;
    }
}
