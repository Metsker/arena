using __Scripts.Generic.Utils.Timer;
namespace Arena.__Scripts.Core.Effects
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
