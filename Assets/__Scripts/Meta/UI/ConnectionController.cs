using __Scripts.Assemblies.Utilities.Debuging;
using Generated;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
namespace Arena.__Scripts.Core.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class ConnectionController : MonoBehaviour
    {
        private UIDocument _uiDocument;

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
            NetworkManager.Singleton.OnClientConnectedCallback += SingletonOnOnClientConnectedCallback;
        }

        private void OnDisable()
        {
            if (NetworkManager.Singleton == null)
                return;
            
            NetworkManager.Singleton.OnClientConnectedCallback -= SingletonOnOnClientConnectedCallback;
        }

        private void SingletonOnOnClientConnectedCallback(ulong id)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;
            
            if (id == NetworkManager.ServerClientId)
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
