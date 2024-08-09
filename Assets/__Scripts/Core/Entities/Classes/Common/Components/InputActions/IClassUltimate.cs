using Unity.Netcode;

namespace Tower.Core.Entities.Classes.Common.Components.InputActions
{
    public interface IClassUltimate
    {
        NetworkVariable<int> UltMeter { get; }
        int MaxStacks { get; }
        float Duration { get; }
        void StackUltimate(int value);
    }
}
