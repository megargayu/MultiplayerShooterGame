using UnityEngine;
using Mirror;
using UnityEngine.UI;

namespace MirrorNetworkConnect
{
    public class AutoHostClient : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private Button hostButton;
        [SerializeField] private GameObject cantHostServerText;

        private void Start()
        {
            if (!Application.isBatchMode) // Headless build (server)
            {
                Debug.Log("=== Client Build ===");

                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    // Can't run server on WebGL!
                    hostButton.interactable = false;
                    cantHostServerText.SetActive(true);
                }

                networkManager.StartClient();
            }
            else
            {
                Debug.Log("=== Server Build ===");
            }
        }

        public void JoinLocal()
        {
            Debug.Log("joining local");
            networkManager.networkAddress = "localhost";
            networkManager.StartClient();
        }
    }
}