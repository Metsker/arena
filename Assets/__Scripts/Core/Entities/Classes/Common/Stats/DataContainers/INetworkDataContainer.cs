using Tower.Core.Entities.Common.Interfaces;

namespace Tower.Core.Entities.Classes.Common.Stats.DataContainers
{
    public interface INetworkDataContainer : IHealth
    {
        int Damage { get; }
        float Speed { get; }
        float AttacksCd { get; }
        float AttackRange { get; }
        void AddSpeed(float amount);
        void SetSpeed(float value);
        void AddAttackSpeed(float amount);
        void AddDamage(int amount);
    }
}
