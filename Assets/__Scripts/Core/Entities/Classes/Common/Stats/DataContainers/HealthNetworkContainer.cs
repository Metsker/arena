using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Unity.Netcode;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Stats.DataContainers
{
    public class HealthNetworkContainer : BaseNetworkHealth
    {
        [Min(0)] [SerializeField] private int initialHealth;
        [Min(0)] [SerializeField] private int initialMaxHealth;
        
        [HideInInspector] public NetworkVariable<int> currentHealth = new ();
        [HideInInspector] public NetworkVariable<int> maxHealth = new ();

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
                return;
            
            SetCurrentHealth(initialHealth);
            SetMaxHealth(initialMaxHealth);
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
