using System;
using Tower.Core.Entities.Classes.Common.Data.Player;
using Tower.Core.Entities.Classes.Common.Stats.SO;
using Tower.Core.Entities.Common.Components;
using Tower.Core.Entities.Common.Data;
using Unity.Netcode;
using UnityEngine;

namespace Tower.Core.Entities.Classes.Common.Stats.DataContainers
{
    /// <summary>
    /// Contains data that can be synced over the network, don't forget to mark network variable dirty if changing it members
    /// </summary>
    public abstract class ClassDataContainer<T> : BaseNetworkHealth, IClassDataContainer where T : ClassData
    {
        private const float MinSpeed = 0.5f;
        private const float MaxSpeed = 2;

        private const float MinAttacksPerSec = 0.5f;
        private const float MaxAttacksPerSec = 5;
        
        [SerializeField] private SyncableData<T> syncableData;
        [SerializeField] private SyncableClassStaticData classStaticData;
        
        public ActionMapData ActionMapData => ClassData.actionMapData;
        public ClassStaticData ClassStaticData => classStaticData.AvailableData;
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
            MarkVarDirty();
        }

        public void SetSpeed(float value)
        {
            if (!IsServer)
                throw new NotServerException();
            
            ClassData.baseStats.speed = value;
            MarkVarDirty();
        }

        public void AddAttackSpeed(float amount)
        {
            if (!IsServer)
                throw new NotServerException();
            
            float attacksPerSec = AttacksPerSec;
            attacksPerSec += amount;
            attacksPerSec = Mathf.Clamp(attacksPerSec, MinAttacksPerSec, MaxAttacksPerSec);
            
            ClassData.baseStats.attacksPerSec = attacksPerSec;
            MarkVarDirty();
        }

        public void AddDamage(int amount)
        {
            if (!IsServer)
                throw new NotServerException();
            
            ClassData.baseStats.damage += amount;
            MarkVarDirty();
        }

        protected override int GetCurrentHealth() =>
            ClassData.baseStats.health;

        protected override void SetCurrentHealth(int value)
        {
            if (!IsServer)
                throw new NotServerException();
            
            ClassData.baseStats.health = value;
            MarkVarDirty();
        }

        protected override int GetMaxHealth() =>
            ClassData.baseStats.maxHealth;

        protected override void SetMaxHealth(int value)
        {
            if (!IsServer)
                throw new NotServerException();
            
            ClassData.baseStats.maxHealth = value;
            MarkVarDirty();
        }

        private void MarkVarDirty() =>
            Data.SetDirty(true);
    }

    public class NotServerException : Exception
    {
        public NotServerException() : base("Cannot modify data on client!")
        {
        }
    }
}
