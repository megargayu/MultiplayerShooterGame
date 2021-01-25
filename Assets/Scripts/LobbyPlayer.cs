using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System.Collections.Generic;
using System.Collections;

namespace MirrorNetworkConnect
{
    public class LobbyPlayer : NetworkBehaviour
    {
        public static LobbyPlayer localPlayer;

        [SyncVar] public string matchID;
        [SyncVar] public int playerIndex;
        [SyncVar] public string username;
        [SyncVar] public bool isHost = false;
        [SyncVar] public bool sceneLoaded = false;

        [SerializeField] private List<Behaviour> nonLobbyPlayerBehaviours;
        [SerializeField] private List<GameObject> nonLobbyPlayerGameObjects;

        private NetworkMatchChecker networkMatchChecker;

        private GameObject playerLobbyUI;

        private void Awake()
        {
            networkMatchChecker = GetComponent<NetworkMatchChecker>();
            DontDestroyOnLoad(gameObject);
        }

        public override void OnStartClient()
        {
            if (isLocalPlayer)
            {
                localPlayer = this;
                CmdSetNonLobbyGameObjects(false);
            }
            else
            {
                Debug.Log("Spawning other player UI");
                playerLobbyUI = UILobby.instance.SpawnPlayerUIPrefab(this, username);
            }
        }

        public override void OnStopClient()
        {
            Debug.Log("Client stopped");
            ClientDisconnect();
        }

        public override void OnStopServer()
        {
            Debug.Log("Client stopped on server");
            ServerDisconnect();
        }

        #region Host Game

        public void HostGame(string username, bool publicMatch)
        {
            string matchID = MatchMaker.GetRandomMatchID();
            CmdHostGame(matchID, username, publicMatch);
        }

        [Command]
        private void CmdHostGame(string matchID, string username, bool publicMatch)
        {
            this.matchID = matchID;
            this.username = username;
            if (MatchMaker.instance.HostGame(matchID, gameObject, publicMatch, out playerIndex))
            {
                Debug.Log("<color=green>Game hosted successfully!</color>");
                networkMatchChecker.matchId = matchID.ToGuid();
                TargetHostGame(true, matchID, username, playerIndex);
            }
            else
            {
                Debug.Log("<color=red>Game hosting failed!</color>");
                TargetHostGame(false, matchID, username, playerIndex);
            }
        }

        [TargetRpc]
        private void TargetHostGame(bool success, string matchID, string username, int playerIndex)
        {
            this.playerIndex = playerIndex;
            this.matchID = matchID;
            UILobby.instance.HostSuccess(success, matchID, username);
        }

        #endregion Host Game

        #region Join Game

        public void JoinGame(string inputID, string username)
        {
            CmdJoinGame(inputID, username);
        }

        [Command]
        private void CmdJoinGame(string matchID, string username)
        {
            this.matchID = matchID;
            this.username = username;
            if (MatchMaker.instance.JoinGame(matchID, gameObject, out playerIndex))
            {
                Debug.Log("<color=green>Game joined successfully!</color>");
                networkMatchChecker.matchId = matchID.ToGuid();
                TargetJoinGame(true, matchID, username, playerIndex);
            }
            else
            {
                Debug.Log("<color=red>Game joining failed!</color>");
                TargetJoinGame(false, matchID, username, playerIndex);
            }
        }

        [TargetRpc]
        private void TargetJoinGame(bool success, string matchID, string username, int playerIndex)
        {
            this.matchID = matchID;
            this.playerIndex = playerIndex;
            UILobby.instance.JoinSuccess(success, matchID, username);
        }

        #endregion Join Game

        #region Search Game

        public void SearchGame(string username)
        {
            CmdSearchGame(username);
        }

        [Command]
        public void CmdSearchGame(string username)
        {
            this.username = username;
            if (MatchMaker.instance.SearchGame(gameObject, out playerIndex, out matchID))
            {
                Debug.Log("<color=green>Game found!</color>");
                networkMatchChecker.matchId = matchID.ToGuid();
                TargetSearchGame(true, matchID, username, playerIndex);
            }
            else
            {
                Debug.Log("<color=red>Game not found!</color>");
                TargetSearchGame(false, matchID, username, playerIndex);
            }
        }

        [TargetRpc]
        public void TargetSearchGame(bool success, string matchID, string username, int playerIndex)
        {
            this.matchID = matchID;
            this.playerIndex = playerIndex;
            UILobby.instance.SearchSuccess(success, matchID, username);
        }

        #endregion Search Game

        #region Begin Game

        public void BeginGame()
        {
            CmdBeginGame();
            CmdSetNonLobbyGameObjects(true);
        }

        [Command]
        private void CmdBeginGame()
        {
            MatchMaker.instance.BeginGame(matchID);
            Debug.Log("<color=red>Game Beginning!</color>");
        }

        public void StartGame()
        {
            TargetBeginGame();
        }

        [TargetRpc]
        private void TargetBeginGame()
        {
            Debug.Log("MatchID: " + matchID + " | Beginning game");
            StartCoroutine(LoadScene());
        }

        private IEnumerator LoadScene()
        {
            AsyncOperation loadScene = SceneManager.LoadSceneAsync(2);
            while (!loadScene.isDone)
            {
                yield return null;
            }

            CmdSetNonLobbyGameObjects(true);
            CmdSetSceneLoaded();
        }

        [Command]
        private void CmdSetSceneLoaded()
        {
            sceneLoaded = true;
        }

        [Command]
        private void CmdSetNonLobbyGameObjects(bool enabled)
        {
            RpcSetNonLobbyGameObjects(enabled);
        }

        [ClientRpc]
        private void RpcSetNonLobbyGameObjects(bool enabled)
        {
            foreach (Behaviour behaviour in nonLobbyPlayerBehaviours)
            {
                behaviour.enabled = enabled;
            }

            foreach (GameObject gObject in nonLobbyPlayerGameObjects)
            {
                gObject.SetActive(enabled);
            }
        }

        #endregion Begin Game

        #region Disconnect Game

        public void DisconnectGame()
        {
            CmdDisconnectGame();
        }

        [Command]
        private void CmdDisconnectGame()
        {
            ServerDisconnect();
        }

        private void ServerDisconnect()
        {
            MatchMaker.instance.PlayerDisconnected(this, matchID);
            networkMatchChecker.matchId = System.Guid.Empty;
            RpcDisconnectGame();
        }

        [ClientRpc]
        private void RpcDisconnectGame()
        {
            ClientDisconnect();
        }

        private void ClientDisconnect()
        {
            if (playerLobbyUI != null)
            {
                Destroy(playerLobbyUI);
            }

            // TODO: When player leaves in match, match ends
            networkMatchChecker.matchId = System.Guid.Empty;
        }

        [TargetRpc]
        public void TargetBecameHost()
        {
            UILobby.instance.SetHost();
        }

        #endregion Disconnect Game
    }
}