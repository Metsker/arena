using Unity.Netcode;

namespace Assemblies.Network
{
    public class TickTimer
    {
        public bool IsRunning => _time > 0;
        
        private float _initialTime;
        private readonly NetworkManager _networkManager;
        private readonly float _delta;
        
        private float _time;

        public TickTimer(NetworkManager networkManager)
        {
            _networkManager = networkManager;
            _delta = _networkManager.LocalTime.FixedDeltaTime;
        }

        public void Start(float sec)
        {
            if (IsRunning)
                return;

            _initialTime = sec;
            _time = _initialTime;
            _networkManager.NetworkTickSystem.Tick += OnTick;
        }

        public void Reset()
        {
            _time = _initialTime;
        }
        
        public void Stop()
        {
            _time = 0;
            _networkManager.NetworkTickSystem.Tick -= OnTick;
        }

        private void OnTick()
        {
            if (!IsRunning)
                return;
            
            _time -= _delta;
            
            if (_time <= 0)
                Stop();
        }
    }
}
