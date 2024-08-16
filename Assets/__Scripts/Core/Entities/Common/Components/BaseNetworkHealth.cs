using System;
using Tower.Core.Entities.Common.Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace Tower.Core.Entities.Common.Components
{
    public abstract class BaseNetworkHealth : NetworkBehaviour, IHealth
    {
        public int CurrentHealth {
            get => GetCurrentHealth();
            private set
            {
                int prevValue = CurrentHealth;
                SetCurrentHealthRpc(value);
                InformAboutHealthChangeRpc(prevValue, value);
            }
        }
        public int MaxHealth 
        {
            get => GetMaxHealth();
            private set => SetMaxHealthRpc(value);
        }

        public GameObject Actor => gameObject;

        public bool FullHealth => CurrentHealth == MaxHealth;
        public bool Dead => CurrentHealth <= 0;
        public float RemainingHeathNormalized => (float)CurrentHealth / MaxHealth;

        public event Action HealthDepleted;
        public event Action<int, int> HealthChanged;
        
        [Rpc(SendTo.Server)]
        public void DealDamageRpc(int amount)
        {
            if (amount <= 0 || CurrentHealth <= 0)
                return;
            
            int newValue = CurrentHealth - amount;
            CurrentHealth = Mathf.Clamp(newValue, 0, MaxHealth);
            
            if (CurrentHealth == 0)
                HealthDepleted?.Invoke();
        }

        [Rpc(SendTo.Server)]
        public void DealDamageRpc(float percent)
        {
            if (percent <= 0 || CurrentHealth <= 0)
                return;
            
            int newValue = CurrentHealth - PercentageDamage(percent);
            
            CurrentHealth = Mathf.Clamp(newValue, 0, MaxHealth);
            
            if (CurrentHealth == 0)
                HealthDepleted?.Invoke();
        }

        protected virtual int PercentageDamage(float percent) =>
            Mathf.CeilToInt(MaxHealth * 0.01f * percent);

        [Rpc(SendTo.Server)]
        public void HealRpc(int amount)
        {
            if (amount <= 0 || FullHealth)
                return;
            
            int newValue = CurrentHealth + amount;
            CurrentHealth = Mathf.Clamp(newValue, 0, MaxHealth);
        }

        [Rpc(SendTo.Server)]
        public void FullHealRpc() =>
            CurrentHealth = MaxHealth;

        [Rpc(SendTo.Server)]
        public void AddMaxHealthRpc(int amount)
        {
            if (amount == 0)
                return;
            
            int result = MaxHealth + amount;
            
            result = Mathf.Clamp(result, 0, int.MaxValue);
            MaxHealth = result;

            if (amount > 0)
                HealRpc(amount);
            else
                DealDamageRpc(amount);
            
        }

        [Rpc(SendTo.Server)]
        public void KillRpc()
        {
            CurrentHealth = 0;
            HealthDepleted?.Invoke();
        }

        [Rpc(SendTo.Everyone)]
        private void InformAboutHealthChangeRpc(int oldValue, int newValue) =>
            HealthChanged?.Invoke(oldValue, newValue);
        
        protected abstract int GetCurrentHealth();
        protected abstract void SetCurrentHealthRpc(int value);
        protected abstract int GetMaxHealth();
        protected abstract void SetMaxHealthRpc(int value);
    }
}
