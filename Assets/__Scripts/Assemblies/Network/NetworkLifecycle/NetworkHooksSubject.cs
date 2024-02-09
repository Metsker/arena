using System.Collections.Generic;
using System.Linq;
using __Scripts.Core.Network.PlayerNetworkLoop.Interfaces;
using Unity.Netcode;
namespace __Scripts.Assemblies.Network.NetworkLifecycle
{
    public class NetworkHooksSubject : NetworkBehaviour
    {
        private readonly HashSet<INetworkLifecycleObserver> _observers = new ();
        private readonly HashSet<INetworkLifecycleOwnerObserver> _ownerObservers = new ();

        public void Register(INetworkObserver observer)
        {
            if (observer is INetworkLifecycleObserver networkObserver)
                _observers.Add(networkObserver);
            
            if (observer is INetworkLifecycleOwnerObserver ownerObserver)
                _ownerObservers.Add(ownerObserver);
        }

        public void Unregister(INetworkObserver observer)
        {
            if (observer is INetworkLifecycleObserver networkObserver)
                _observers.Remove(networkObserver);
            
            if (observer is INetworkLifecycleOwnerObserver ownerObserver)
                _ownerObservers.Remove(ownerObserver);
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                foreach (INetworkLifecycleOwnerObserver ownerObserver in _ownerObservers)
                    ownerObserver.OnNetworkSpawnOwner(this);
            }
            else
            {
                foreach (INetworkLifecycleObserver observer in _observers)
                    observer.OnNetworkSpawn(this);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner)
            {
                foreach (INetworkLifecycleOwnerObserver ownerObserver in _ownerObservers)
                    ownerObserver.OnNetworkDespawnOwner(this);
            }
            else
            {
                foreach (INetworkLifecycleObserver observer in _observers)
                    observer.OnNetworkDespawn(this);
            }
        }
    }
}
