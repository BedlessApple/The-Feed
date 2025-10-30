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
        SceneManager.LoadScene("GameScene"); // Replace with your actual scene name
    }

    private void ClientButtonOnClick()
    {
        NetworkManager.Singleton.StartClient();
        SceneManager.LoadScene("GameScene"); // Replace with your actual scene name
    }
}
