using Tower.Core.Entities.Classes.Common.Stats.SO;
using Tower.Core.Entities.Common.Components;
using Tower.Core.Entities.Enemies.Common;
using Unity.Netcode;
using UnityEngine;

namespace Tower.Core.Entities.Enemies.Bosses.Gargoyle.Data
{
    public abstract class BossDataContainer<T> : BaseNetworkHealth, IBossDataContainer where T : BossData
    {
        [SerializeField] private SyncableData<T> syncableData;

        public NetworkVariable<int> StageIndex { get; } = new (writePerm: NetworkVariableWritePermission.Server);
        public Stage CurrentStage => Data.Value.stages[StageIndex.Value];

        protected readonly NetworkVariable<T> Data = new ();
        
        public override void OnNetworkSpawn()
        {
            if (IsServer)
                Data.Value = syncableData.CopyData();
        }
        
        public void ToNextStage() =>
            StageIndex.Value += 1;

        protected override int GetCurrentHealth() =>
            Data.Value.health;

        [Rpc(SendTo.Server)]
        protected override void SetCurrentHealthRpc(int value)
        {
            Data.Value.health = value;
            
            int nextStageIndex = StageIndex.Value + 1;
            if (Data.Value.stages.Length > nextStageIndex && RemainingHeathNormalized <= Data.Value.stages[nextStageIndex].startHealthNm)
                ToNextStage();
            
            Data.SetDirty(true);
        }

        protected override int GetMaxHealth() =>
            Data.Value.maxHealth;

        [Rpc(SendTo.Server)]
        protected override void SetMaxHealthRpc(int value)
        {
            Data.Value.maxHealth = value;
            Data.SetDirty(true);
        }
    }
}
