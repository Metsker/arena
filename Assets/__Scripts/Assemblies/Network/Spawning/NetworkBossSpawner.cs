using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assemblies.Network.Spawning
{
    public class NetworkBossSpawner : MonoBehaviour
    {
        [SerializeField] private NetworkObject bossPrefab;
        
        private void Awake() =>
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;

        private void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
        }

        private void OnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;
            
            NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(bossPrefab);
        }
    }
}
