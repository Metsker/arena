using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class ServerController : MonoBehaviour
{
    private void Awake()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();

        uiDocument.rootVisualElement.Q<Button>("Server")
            .clickable.clicked += ServerOnClick;

        uiDocument.rootVisualElement.Q<Button>("Client")
            .clickable.clicked += ClientOnClick;

        uiDocument.rootVisualElement.Q<Button>("Host")
            .clickable.clicked += HostOnClick;
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
