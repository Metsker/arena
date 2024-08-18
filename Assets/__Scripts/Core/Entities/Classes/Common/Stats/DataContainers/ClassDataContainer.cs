using Tower.Core.Entities.Classes.Common.Data.Player;
using Tower.Core.Entities.Classes.Common.Stats.SO;
using Tower.Core.Entities.Common.Components;
using Tower.Core.Entities.Common.Data;
using Unity.Netcode;
using UnityEngine;
using VContainer;

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

        public ActionMapData ActionMapData => ClassData.actionMapData;
        public ClassStaticData ClassStaticData => _classStaticData;
        public float AttacksCd => 1 / AttacksPerSec;
        public float Speed => ClassData.baseStats.speed;
        public int Damage => ClassData.baseStats.damage;
        public float AttackRange => ClassData.baseStats.attackRange;
        private float AttacksPerSec => ClassData.baseStats.attacksPerSec;

        private ClassData ClassData => Data.Value;

        protected readonly NetworkVariable<T> Data = new ();
        
        private ClassStaticData _classStaticData;

        [Inject]
        private void Construct(ClassStaticData classStatic)
        {
            _classStaticData = classStatic;
        }
        
        public override void OnNetworkSpawn()
        {
            if (IsServer)
                Data.Value = syncableData.CopyData();
        }

        [Rpc(SendTo.Server)]
        public void AddSpeedRpc(float amount)
        {
            float speed = Speed;
            speed += amount;
            speed = Mathf.Clamp(speed, MinSpeed, MaxSpeed);
            
            ClassData.baseStats.speed = speed;
            MarkVarDirty();
        }

        [Rpc(SendTo.Server)]
        public void SetSpeedRpc(float value)
        { 
            ClassData.baseStats.speed = value;
            MarkVarDirty();
        }

        [Rpc(SendTo.Server)]
        public void AddAttackSpeedRpc(float amount)
        {
            float attacksPerSec = AttacksPerSec;
            attacksPerSec += amount;
            attacksPerSec = Mathf.Clamp(attacksPerSec, MinAttacksPerSec, MaxAttacksPerSec);
            
            ClassData.baseStats.attacksPerSec = attacksPerSec;
            MarkVarDirty();
        }

        [Rpc(SendTo.Server)]
        public void AddDamageRpc(int amount)
        {
            ClassData.baseStats.damage += amount;
            MarkVarDirty();
        }

        protected override int GetCurrentHealth() =>
            ClassData.baseStats.health;

        [Rpc(SendTo.Server)]
        protected override void SetCurrentHealthRpc(int value)
        {
            ClassData.baseStats.health = value;
            MarkVarDirty();
        }

        protected override int GetMaxHealth() =>
            ClassData.baseStats.maxHealth;

        [Rpc(SendTo.Server)]
        protected override void SetMaxHealthRpc(int value)
        {
            ClassData.baseStats.maxHealth = value;
            MarkVarDirty();
        }

        private void MarkVarDirty() =>
            Data.SetDirty(true);
    }
}
