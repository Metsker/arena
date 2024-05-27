using System;

namespace __Scripts.Assemblies.Utilities.Timers
{
    public abstract class Timer
    {
        protected float initialTime;
        protected float Time { get; set; }
        public bool IsRunning { get; protected set; }

        public float Progress => Time / initialTime;

        public Action OnTimerStart = delegate {};
        public Action OnTimerStop = delegate {};

        protected Timer(float value)
        {
            initialTime = value;
            IsRunning = false;
        }

        public void Start(float newValue = 0)
        {
            Time = newValue == 0 ? initialTime : newValue;
            
            if (!IsRunning)
            {
                IsRunning = true;
                OnTimerStart.Invoke();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                OnTimerStop.Invoke();
            }
        }

        public void ResetEvents()
        {
            OnTimerStart = delegate {};
            OnTimerStop = delegate {};
        }
        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;

        public abstract void Tick(float deltaTime);
    }

}
