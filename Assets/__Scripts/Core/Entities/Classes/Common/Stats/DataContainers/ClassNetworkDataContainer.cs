using System;
using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Arena.__Scripts.Core.Entities.Classes.Common.Data.Player;
using Arena.__Scripts.Core.Entities.Classes.Common.Stats.SO;
using Unity.Netcode;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Stats.DataContainers
{
    public abstract class ClassNetworkDataContainer<T> : BaseNetworkHealth, IClassNetworkDataContainer where T : ClassData
    {
        private const float MinSpeed = 0.5f;
        private const float MaxSpeed = 2;

        private const float MinAttacksPerSec = 0.5f;
        private const float MaxAttacksPerSec = 5;
        
        [SerializeField] private SyncableData<T> syncableData;
        
        public ActionMapData ActionMapData => ClassData.actionMapData;
        public float AttacksCd => 1 / AttacksPerSec;
        public float Speed => ClassData.baseStats.speed;
        public int Damage => ClassData.baseStats.damage;
        public float AttackRange => ClassData.baseStats.attackRange;
        private float AttacksPerSec => ClassData.baseStats.attacksPerSec;
        
        private ClassData ClassData => Data.Value;

        protected readonly NetworkVariable<T> Data = new ();

        public override void OnNetworkSpawn()
        {
            if (IsServer)
                Data.Value = syncableData.CopyData();
        }

        public void AddSpeed(float amount)
        {
            if (!IsServer)
                throw new NotServerException();
            
            float speed = Speed;
            speed += amount;
            speed = Mathf.Clamp(speed, MinSpeed, MaxSpeed);
            
            ClassData.baseStats.speed = speed;
            MarkAsDirty();
        }

        public void SetSpeed(float value)
        {
            if (!IsServer)
                throw new NotServerException();
            
            ClassData.baseStats.speed = value;
            MarkAsDirty();
        }

        public void AddAttackSpeed(float amount)
        {
            if (!IsServer)
                throw new NotServerException();
            
            float attacksPerSec = AttacksPerSec;
            attacksPerSec += amount;
            attacksPerSec = Mathf.Clamp(attacksPerSec, MinAttacksPerSec, MaxAttacksPerSec);
            
            ClassData.baseStats.attacksPerSec = attacksPerSec;
            MarkAsDirty();
        }

        public void AddDamage(int amount)
        {
            if (!IsServer)
                throw new NotServerException();
            
            ClassData.baseStats.damage += amount;
            MarkAsDirty();
        }

        protected override int GetCurrentHealth() =>
            ClassData.baseStats.health;

        protected override void SetCurrentHealth(int value)
        {
            if (!IsServer)
                throw new NotServerException();
            
            ClassData.baseStats.health = value;
            MarkAsDirty();
        }

        protected override int GetMaxHealth() =>
            ClassData.baseStats.maxHealth;

        protected override void SetMaxHealth(int value)
        {
            if (!IsServer)
                throw new NotServerException();
            
            ClassData.baseStats.maxHealth = value;
            MarkAsDirty();
        }

        private void MarkAsDirty() =>
            Data.SetDirty(true);
    }

    public class NotServerException : Exception
    {
        public NotServerException() : base("Cannot modify data on client!")
        {
        }
    }
}
