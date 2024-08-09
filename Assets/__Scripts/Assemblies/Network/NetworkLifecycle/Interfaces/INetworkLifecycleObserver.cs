using Unity.Netcode;

namespace Assemblies.Network.NetworkLifecycle.Interfaces
{
    public interface INetworkLifecycleObserver : INetworkOwnershipObserver
    {
        void OnNetworkSpawn(NetworkBehaviour networkBehaviour);
        void OnNetworkDespawn(NetworkBehaviour networkBehaviour);
    }
}
