using __Scripts.Generic.Utils.Timer;
namespace Arena.__Scripts.Core.Effects
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
