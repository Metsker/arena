namespace Arena.__Scripts.Shared.Utils.Timer
{
    public class CountdownTimer : Timer
    {
        public CountdownTimer(float value) : base(value) {}

        public override void Tick(float deltaTime)
        {
            if (IsRunning && Time > 0)
            {
                Time -= deltaTime;
            }

            if (IsRunning && Time <= 0)
            {
                Stop();
            }
        }

        public bool IsFinished => Time <= 0;

        public void Reset() => Time = initialTime;
        
        public float GetTime() => Time;

        public void Reset(float newTime)
        {
            initialTime = newTime;
            Reset();
        }
    }
}
