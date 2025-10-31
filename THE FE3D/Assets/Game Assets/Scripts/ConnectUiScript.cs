using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConnectUiScript : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    void Start()
    {
        hostButton.onClick.AddListener(HostButtonOnClick);
        clientButton.onClick.AddListener(ClientButtonOnClick);
    }

    private void HostButtonOnClick()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    private void ClientButtonOnClick()
    {
        NetworkManager.Singleton.StartClient();
        // Clients do not manually load the scene — host syncs it
    }
}
