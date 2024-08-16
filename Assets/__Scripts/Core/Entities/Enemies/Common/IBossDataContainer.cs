using Unity.Netcode;

namespace Tower.Core.Entities.Enemies.Common
{
    public interface IBossDataContainer
    {
        NetworkVariable<int> StageIndex { get; } 
        Stage CurrentStage { get; }
        public void ToNextStage();
    }
}
