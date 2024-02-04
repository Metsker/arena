using Arena.__Scripts.Generic.Effects;
using Arena.__Scripts.Shared.Utils.Timer;
namespace Arena.__Scripts.General.Effects
{
    public abstract class Debuff : IEffect
    {
        public CountdownTimer Timer { get; set; }

        protected Debuff(float duration)
        {
            Timer = new CountdownTimer(duration);
        }
        public virtual void OnApply() {}
        public virtual void OnTimeOut(){}
        public virtual void OnTick(float remainingTime){}
    }
}
