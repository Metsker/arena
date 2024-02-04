using System;
using Unity.Netcode;
namespace Arena.__Scripts.Core.Entities.Classes.Shared.Components
{
    public class NetworkHooks : NetworkBehaviour
    {
        public event Action NetworkSpawned;
        public event Action NetworkOwnerSpawned;
        
        public event Action NetworkDespawned;
        public event Action NetworkOwnerDespawned;
        
        public override void OnNetworkSpawn()
        {
            NetworkSpawned?.Invoke();
            
            if (IsOwner)
                NetworkOwnerSpawned?.Invoke();
        }

        public override void OnNetworkDespawn()
        {
            NetworkDespawned?.Invoke();
            
            if (IsOwner)
                NetworkOwnerDespawned?.Invoke();
        }
    }
}
