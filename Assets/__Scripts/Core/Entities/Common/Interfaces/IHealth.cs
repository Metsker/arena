using System;
using UnityEngine;

namespace Tower.Core.Entities.Common.Interfaces
{
    public interface IHealth
    {
        public int CurrentHealth { get; }
        public int MaxHealth { get; }
        public bool FullHealth { get; }
        public bool Dead { get; }
        public GameObject Actor { get; }
        float RemainingHeathNormalized { get; }
        public event Action HealthDepleted;
        public event Action<int, int> HealthChanged;
        void DealDamageRpc(int amount);
        void DealDamageRpc(float percent);
        void HealRpc(int amount);
        void KillRpc();
        void FullHealRpc();
        void AddMaxHealthRpc(int amount);
    }
}
