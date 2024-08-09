namespace Assemblies.Utilities.Timers
{
    public class CountdownTimer : Timer
    {
        public float Progress => Time / initialTime;
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
