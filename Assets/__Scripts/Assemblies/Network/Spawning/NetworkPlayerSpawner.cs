using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace __Scripts.Assemblies.Network.Spawning
{
    public class NetworkPlayerSpawner : MonoBehaviour
    {
        [SerializeField] private NetworkObject playerPrefab;

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
            
            foreach (ulong client in clientscompleted)
            {
                NetworkObject playerObj = Instantiate(playerPrefab);
                playerObj.SpawnAsPlayerObject(client, true);
            }
        }
    }
}
