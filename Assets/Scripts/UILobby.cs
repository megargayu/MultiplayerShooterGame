using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

namespace MirrorNetworkConnect
{
    public class UILobby : MonoBehaviour
    {
        public static UILobby instance;

        [Header("Host Join")]
        [SerializeField] private TMP_InputField joinMatchInput;

        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private List<Selectable> lobbySelectables = new List<Selectable>();
        [SerializeField] private GameObject lobbyImage;
        [SerializeField] private GameObject searchImage;
        [SerializeField] private TextMeshProUGUI errorText;

        [Header("Lobby")]
        [SerializeField] private Transform UIPlayerParent;

        [SerializeField] private GameObject UIPlayerPrefab;
        [SerializeField] private TextMeshProUGUI matchIDText;
        [SerializeField] private GameObject beginGameButton;

        private GameObject playerLobbyUI;

        private bool searching = false;

        private class Errors
        {
            public const string invalidUsername = "Invalid Username!";
            public const string usernameTaken = "Username Already Taken!";
        }

        private void Start()
        {
            instance = this;
        }

        private bool CheckUsername()
        {
            if (usernameInput.text.Length < 3)
            {
                errorText.SetText(Errors.invalidUsername);
                return false;
            }
            else if (MatchMaker.instance.UsernameExists(usernameInput.text))
            {
                errorText.SetText(Errors.usernameTaken);
                return false;
            }
            else if (errorText.text == Errors.invalidUsername || errorText.text == Errors.usernameTaken)
            {
                errorText.SetText(string.Empty);
            }

            return true;
        }

        public void HostPrivate()
        {
            if (!CheckUsername()) return;
            joinMatchInput.interactable = false;
            lobbySelectables.ForEach(x => x.interactable = false);

            LobbyPlayer.localPlayer.isHost = true;
            LobbyPlayer.localPlayer.HostGame(usernameInput.text, false);
        }

        public void HostPublic()
        {
            if (!CheckUsername()) return;
            joinMatchInput.interactable = false;
            lobbySelectables.ForEach(x => x.interactable = false);

            LobbyPlayer.localPlayer.isHost = true;
            LobbyPlayer.localPlayer.HostGame(usernameInput.text, true);
        }

        public void HostSuccess(bool success, string matchID, string username)
        {
            if (success)
            {
                lobbyImage.SetActive(true);

                if (playerLobbyUI != null) Destroy(playerLobbyUI);
                playerLobbyUI = SpawnPlayerUIPrefab(LobbyPlayer.localPlayer, username);
                matchIDText.text = "<size=20><color=#212121>MATCH ID:</color></size>\n" + matchID;
                beginGameButton.SetActive(true);
            }
            else
            {
                joinMatchInput.interactable = true;
                lobbySelectables.ForEach(x => x.interactable = true);
            }
        }

        public void SetHost()
        {
            beginGameButton.SetActive(true);
        }

        public void Join()
        {
            if (!CheckUsername()) return;
            joinMatchInput.interactable = false;
            lobbySelectables.ForEach(x => x.interactable = false);

            LobbyPlayer.localPlayer.JoinGame(joinMatchInput.text, usernameInput.text);
        }

        public void JoinSuccess(bool success, string matchID, string username)
        {
            if (success)
            {
                lobbyImage.SetActive(true);

                if (playerLobbyUI != null) Destroy(playerLobbyUI);
                playerLobbyUI = SpawnPlayerUIPrefab(LobbyPlayer.localPlayer, username);
                matchIDText.text = "<size=20><color=#212121>MATCH ID:</color></size>\n" + matchID;
                beginGameButton.SetActive(false);
            }
            else
            {
                joinMatchInput.interactable = true;
                lobbySelectables.ForEach(x => x.interactable = true);
            }
        }

        public GameObject SpawnPlayerUIPrefab(LobbyPlayer player, string username)
        {
            GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPlayerParent);
            newUIPlayer.GetComponent<UIPlayer>().SetPlayer(player, username);
            newUIPlayer.transform.SetSiblingIndex(player.playerIndex - 1);

            return newUIPlayer;
        }

        public void BeginGame()
        {
            LobbyPlayer.localPlayer.BeginGame();
        }

        public void SearchGame()
        {
            if (!CheckUsername()) return;
            Debug.Log("Searching for game");
            searchImage.SetActive(true);
            StartCoroutine(SearchingForGame());
        }

        private IEnumerator SearchingForGame()
        {
            searching = true;

            float currentTime = 1;
            while (searching)
            {
                if (currentTime > 0)
                {
                    currentTime -= Time.deltaTime;
                }
                else
                {
                    currentTime = 1;
                    LobbyPlayer.localPlayer.SearchGame(usernameInput.text);
                }
                yield return null;
            }
        }

        public void SearchSuccess(bool success, string matchID, string username)
        {
            if (success)
            {
                searchImage.SetActive(false);
                JoinSuccess(success, matchID, username);
                searching = false;
            }
        }

        public void SearchCancel()
        {
            searching = false;
            searchImage.SetActive(false);
            lobbySelectables.ForEach(x => x.interactable = true);
        }

        public void DisconnectLobby()
        {
            if (playerLobbyUI != null) Destroy(playerLobbyUI);

            LobbyPlayer.localPlayer.DisconnectGame();

            lobbyImage.SetActive(false);
            lobbySelectables.ForEach(x => x.interactable = true);
            beginGameButton.SetActive(false);
        }
    }
}