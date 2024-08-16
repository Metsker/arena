using Tower.Core.Entities.Common.Interfaces;

namespace Tower.Core.Entities.Classes.Common.Stats.DataContainers
{
    public interface INetworkDataContainer : IHealth
    {
        int Damage { get; }
        float Speed { get; }
        float AttacksCd { get; }
        float AttackRange { get; }
        void AddSpeedRpc(float amount);
        void SetSpeedRpc(float value);
        void AddAttackSpeedRpc(float amount);
        void AddDamageRpc(int amount);
    }
}
