using System;
using JetBrains.Annotations;
using UnityEngine;
using VContainer.Unity;
namespace Arena.__Scripts.Core.Network
{
    [UsedImplicitly]
    public class NetworkTimer : ITickable
    {
        public float MinTimeBetweenTicks { get; }
        public int CurrentTick { get; private set; }

        public event Action<int> Ticked;
        
        private float _timer;

        public NetworkTimer(float serverTickRate)
        {
            MinTimeBetweenTicks = 1f / serverTickRate;
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
            
            if (_timer < MinTimeBetweenTicks)
                return;

            _timer -= MinTimeBetweenTicks;
            CurrentTick++;
            
            Ticked?.Invoke(CurrentTick);
        }
    }
}
