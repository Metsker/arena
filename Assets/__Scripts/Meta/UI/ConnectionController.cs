using __Scripts.Assemblies.Utilities.Debuging;
using Generated;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Arena.__Scripts.Meta.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class ConnectionController : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private int _playersInRoom;

        private void Awake()
        {
            if (DebugData.IsDebug)
            {
                NetworkManager.Singleton.StartHost();
                LoadCore();
                enabled = false;
                return;
            }
            
            _uiDocument = GetComponent<UIDocument>();

            _uiDocument.rootVisualElement.Q<Button>("Server")
                .clickable.clicked += ServerOnClick;

            _uiDocument.rootVisualElement.Q<Button>("Client")
                .clickable.clicked += ClientOnClick;

            _uiDocument.rootVisualElement.Q<Button>("Host")
                .clickable.clicked += HostOnClick;
        }

        private void OnEnable()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        }

        private void OnDisable()
        {
            if (NetworkManager.Singleton == null)
                return;
            
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        }

        private void OnClientConnectedCallback(ulong id)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;
            
            _playersInRoom++;
            
            if (_playersInRoom < DebugData.PlayersToWait)
                return;
            
            LoadCore();
        }

        private void ServerOnClick()
        {
            //NetworkManager.Singleton.StartServer();
            LoadCore();
        }

        private void ClientOnClick()
        {
            NetworkManager.Singleton.StartClient();
            
        }

        private void HostOnClick()
        {
            NetworkManager.Singleton.StartHost();
        }

        private static void LoadCore() =>
            NetworkManager.Singleton.SceneManager.LoadScene(ScenesInBuild.Core.ToString(), LoadSceneMode.Single);
    }
}
