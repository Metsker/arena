using Arena.__Scripts.Shared.Utils.Timer;
using UnityEngine;
namespace Arena.__Scripts.Generic.Effects
{
    public interface IEffect
    {
        CountdownTimer Timer { get; }
        void OnApply();
        void OnTimeOut();
        //No tick for the first call
        void OnTick(float remainingTime);
    }
}
