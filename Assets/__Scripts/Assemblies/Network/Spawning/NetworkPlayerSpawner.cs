using System;
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

        private void OnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;
            
            foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
            {
                NetworkManager.Singleton.SpawnManager
                    .InstantiateAndSpawn(
                        playerPrefab,
                        client.Key,
                        true, 
                        true);
            }
        }
    }
}
