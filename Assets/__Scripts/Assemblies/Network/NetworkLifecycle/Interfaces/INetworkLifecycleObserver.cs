using Unity.Netcode;
namespace __Scripts.Core.Network.PlayerNetworkLoop.Interfaces
{
    /// <summary>
    /// Please register your observer via NetworkHooksSubject
    /// </summary>
    public interface INetworkLifecycleObserver : INetworkObserver
    {
        void OnNetworkSpawn(NetworkBehaviour networkBehaviour);
        void OnNetworkDespawn(NetworkBehaviour networkBehaviour);
    }
}
