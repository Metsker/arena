namespace Assemblies.Network.NetworkLifecycle.Interfaces
{
    public interface INetworkOwnershipObserver
    {
        bool IsOwner { get; set; }
    }
}
