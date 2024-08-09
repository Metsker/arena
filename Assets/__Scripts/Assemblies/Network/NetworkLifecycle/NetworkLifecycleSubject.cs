using System.Collections.Generic;
using Assemblies.Network.NetworkLifecycle.Interfaces;
using Unity.Netcode;

namespace Assemblies.Network.NetworkLifecycle
{
    public class NetworkLifecycleSubject : NetworkBehaviour
    {
        private readonly HashSet<INetworkOwnershipObserver> _ownershipObservers = new ();
        private readonly HashSet<INetworkLifecycleObserver> _lifecycleObservers = new ();
        private readonly HashSet<INetworkLifecycleOwnerObserver> _lifecycleOwnerObservers = new ();

        public void Register(INetworkOwnershipObserver ownershipObserver) =>
            _ownershipObservers.Add(ownershipObserver);

        public void Register(INetworkLifecycleObserver networkObserver) =>
            _lifecycleObservers.Add(networkObserver);

        public void Register(INetworkLifecycleOwnerObserver ownerObserver) =>
            _lifecycleOwnerObservers.Add(ownerObserver);

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                foreach (INetworkLifecycleOwnerObserver ownerObserver in _lifecycleOwnerObservers)
                {
                    ownerObserver.IsOwner = true;
                    ownerObserver.OnNetworkSpawnOwner(this);
                }
                foreach (INetworkOwnershipObserver ownershipObserver in _ownershipObservers)
                    ownershipObserver.IsOwner = true;
            }
            foreach (INetworkLifecycleObserver observer in _lifecycleObservers)
            {
                observer.IsOwner = IsOwner;
                observer.OnNetworkSpawn(this);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner)
            {
                foreach (INetworkLifecycleOwnerObserver ownerObserver in _lifecycleOwnerObservers)
                    ownerObserver.OnNetworkDespawnOwner(this);
                
                _lifecycleOwnerObservers.Clear();
                _ownershipObservers.Clear();
            }
            foreach (INetworkLifecycleObserver observer in _lifecycleObservers)
                observer.OnNetworkDespawn(this);
                
            _lifecycleObservers.Clear();
        }
    }
}
