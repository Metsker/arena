using System;

namespace Arena.__Scripts.Core.Entities.Common.Interfaces
{
    public interface IHealth
    {
        public int CurrentHealth { get; }
        public int MaxHealth { get; }
        public bool FullHealth { get; }
        public event Action HealthDepleted;
        void DealDamageRpc(int amount);
        void DealDamageRpc(float percent);
        void HealRpc(int amount);
        void KillRpc();
        void FullHealRpc();
        void AddMaxHealthRpc(int amount);
    }
}
