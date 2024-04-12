using System;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Components
{
    public abstract class BaseNetworkHealth : NetworkBehaviour, IHealth
    {
        public int CurrentHealth 
        {
            get => GetCurrentHealth();
            private set => SetCurrentHealth(value);
        }
        public int MaxHealth 
        {
            get => GetMaxHealth();
            private set => SetMaxHealth(value);
        }
        public bool FullHealth => CurrentHealth == MaxHealth;
        public float RemainingHeathNormalized => (float)CurrentHealth / MaxHealth;

        public event Action HealthDepleted;

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

        protected abstract int GetCurrentHealth();
        protected abstract void SetCurrentHealth(int value);
        protected abstract int GetMaxHealth();
        protected abstract void SetMaxHealth(int value);
    }
}
