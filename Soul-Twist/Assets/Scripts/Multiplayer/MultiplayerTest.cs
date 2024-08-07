using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class MultiplayerTest : MonoBehaviour
{
    [SerializeField] private Button startHost;
    [SerializeField] private Button startClient;
    private void Awake()
    {
        startHost.onClick.AddListener(() => {
            gameObject.SetActive(false);
            NetworkManager.Singleton.StartHost();
        });

        startClient.onClick.AddListener(() => {
            gameObject.SetActive(false);
            NetworkManager.Singleton.StartClient();
        });
    }
}
