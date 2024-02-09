using Unity.Netcode;
namespace __Scripts.Core.Network.PlayerNetworkLoop.Interfaces
{
    /// <summary>
    /// Please register your observer via NetworkHooksSubject
    /// </summary>
    public interface INetworkLifecycleOwnerObserver : INetworkObserver
    {
        void OnNetworkSpawnOwner(NetworkBehaviour networkBehaviour);
        void OnNetworkDespawnOwner(NetworkBehaviour networkBehaviour);
    }
}
