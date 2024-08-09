using System;

namespace Assemblies.Utilities.Timers
{
    public abstract class Timer
    {
        protected float initialTime;
        protected float Time { get; set; }
        public bool IsRunning { get; protected set; }

        public Action OnTimerStart = delegate {};
        public Action OnTimerStop = delegate {};

        protected Timer(float value)
        {
            initialTime = value;
            IsRunning = false;
        }

        public void Start(float newValue = 0)
        {
            if (newValue == 0)
                Time = initialTime;
            else
            {
                initialTime = newValue;
                Time = initialTime;
            }

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
