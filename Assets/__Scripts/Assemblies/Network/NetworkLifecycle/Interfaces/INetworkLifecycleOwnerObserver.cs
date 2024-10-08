﻿using Unity.Netcode;

namespace Assemblies.Network.NetworkLifecycle.Interfaces
{
    public interface INetworkLifecycleOwnerObserver : INetworkOwnershipObserver
    {
        void OnNetworkSpawnOwner(NetworkBehaviour networkBehaviour);
        void OnNetworkDespawnOwner(NetworkBehaviour networkBehaviour);
    }
}
