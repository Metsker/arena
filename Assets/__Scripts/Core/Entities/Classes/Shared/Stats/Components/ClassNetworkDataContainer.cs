using System;
using Arena.__Scripts.Core.Entities.Classes.Alchemist;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using Arena.__Scripts.Core.Entities.Classes.Data;
using Arena.__Scripts.Core.Entities.Generic;
using Unity.Netcode;
using UnityEngine;
namespace Arena.__Scripts.Core.Entities.Classes.Shared.Stats.Components
{
    public abstract class ClassNetworkDataContainer : NetworkBehaviour, IHealth
    {
        private const float MinSpeed = 0.5f;
        private const float MaxSpeed = 2;
        
        private const float MinAttacksPerSec = 0.5f;
        private const float MaxAttacksPerSec = 5;

        public float AttacksCd => 1 / attacksPerSec.Value;
        
        public ActionMapData ActionMapData { get; private set; }
        
        public NetworkVariable<float> currentHealth;
        public NetworkVariable<int> maxHealth;
        public NetworkVariable<float> speed;
        public NetworkVariable<int> damage;
        public NetworkVariable<float> attacksPerSec;
        
        public event Action HealthDepleted;

        protected void Init(ClassData classData)
        {
            ActionMapData = classData.actionMapData;
            BaseStats baseStats = classData.baseStats;
            
            currentHealth = new NetworkVariable<float>(baseStats.health);
            maxHealth = new NetworkVariable<int>(baseStats.health);
            speed = new NetworkVariable<float>(baseStats.speed);
            damage = new NetworkVariable<int>(baseStats.damage);
            attacksPerSec = new NetworkVariable<float>(baseStats.attacksPerSec);
        }
        
        public void DealDamage(int amount)
        {
            currentHealth.Value -= amount;
            currentHealth.Value = Mathf.Clamp(currentHealth.Value, 0, maxHealth.Value);

            if (currentHealth.Value == 0)
                HealthDepleted?.Invoke();
        }
        
        public void DealDamage(float percent)
        {
            currentHealth.Value -= maxHealth.Value * percent;
            currentHealth.Value = Mathf.Clamp(currentHealth.Value, 0, maxHealth.Value);

            if (currentHealth.Value == 0)
                HealthDepleted?.Invoke();
        }

        public void Heal(int amount)
        {
            currentHealth.Value += amount;
            currentHealth.Value = Mathf.Clamp(currentHealth.Value, 0, maxHealth.Value);
        }
        
        public void IncreaseMaxHealth(int amount)
        {
            maxHealth.Value += amount;
            
            Heal(amount);
        }

        public void AddSpeed(float amount)
        {
            speed.Value += amount;
            speed.Value = Mathf.Clamp(speed.Value, MinSpeed, MaxSpeed);
        }
        
        public void AddAttackSpeed(float amount)
        {
            attacksPerSec.Value += amount;
            attacksPerSec.Value = Mathf.Clamp(attacksPerSec.Value, MinAttacksPerSec, MaxAttacksPerSec);
        }
        
        public void AddDamage(int amount) =>
            damage.Value += amount;
    }
}
