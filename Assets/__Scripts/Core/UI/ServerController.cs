using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
namespace Arena.__Scripts.Core.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class ServerController : MonoBehaviour
    {
        private UIDocument _uiDocument;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();

            _uiDocument.enabled = true;

            _uiDocument.rootVisualElement.Q<Button>("Server")
                .clickable.clicked += ServerOnClick;

            _uiDocument.rootVisualElement.Q<Button>("Client")
                .clickable.clicked += ClientOnClick;

            _uiDocument.rootVisualElement.Q<Button>("Host")
                .clickable.clicked += HostOnClick;
        }

        private void OnDisable()
        {
            _uiDocument.enabled = false;
        }

        private void ServerOnClick()
        {
            NetworkManager.Singleton.StartServer();
        }

        private void ClientOnClick()
        {
            NetworkManager.Singleton.StartClient();
        }

        private void HostOnClick()
        {
            NetworkManager.Singleton.StartHost();
        }
    }
}
