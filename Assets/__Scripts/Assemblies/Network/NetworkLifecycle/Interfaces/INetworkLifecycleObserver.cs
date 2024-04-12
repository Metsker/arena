using Unity.Netcode;

namespace __Scripts.Assemblies.Network.NetworkLifecycle.Interfaces
{
    public interface INetworkLifecycleObserver : INetworkOwnershipObserver
    {
        void OnNetworkSpawn(NetworkBehaviour networkBehaviour);
        void OnNetworkDespawn(NetworkBehaviour networkBehaviour);
    }
}
