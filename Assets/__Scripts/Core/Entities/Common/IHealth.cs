using System;
namespace Arena.__Scripts.Core.Entities.Generic
{
    public interface IHealth
    {
        public event Action HealthDepleted;
        void DealDamage(int amount);
        void DealDamage(float percent);
        void Heal(int amount);
        void IncreaseMaxHealth(int amount);
    }
}
