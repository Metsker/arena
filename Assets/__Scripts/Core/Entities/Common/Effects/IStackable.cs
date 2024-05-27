using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Common.Effects
{
    public interface IStackable
    {
        [Min(1)] public int Stacks { get; set; }

        void ResetStacks();
    }
}
