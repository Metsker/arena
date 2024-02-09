using __Scripts.Assemblies.Network.Messages;
using Unity.Netcode;
using UnityEngine;

namespace __Scripts.Core.Network
{
    public class NetworkManagerCallbacks : MonoBehaviour
    {
        private NetworkMessageSystem _networkMessageSystem;
        
        private void Start() =>
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
        
        private void OnClientConnect(ulong id)
        {
            if (id != NetworkManager.Singleton.LocalClientId)
                return;
            
            _networkMessageSystem = new NetworkMessageSystem();
        }
    }
}
