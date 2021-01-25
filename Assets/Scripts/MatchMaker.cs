using UnityEngine;
using Mirror;
using System;
using System.Security.Cryptography;
using System.Text;

namespace MirrorNetworkConnect
{
    [Serializable]
    public class Match
    {
        public string matchID;

        public bool publicMatch;
        public bool inMatch;
        public bool matchFull;

        // TODO: Don't use GameObjects (https://mirror-networking.com/docs/Articles/Guides/DataTypes.html#game-objects)
        public SyncListGameObject players = new SyncListGameObject();

        public Match(string matchID, GameObject player)
        {
            this.matchID = matchID;
            players.Add(player);
        }

        public Match()
        {
        }
    }

    [Serializable]
    public class SyncListMatch : SyncList<Match> { }

    [Serializable]
    public class SyncListGameObject : SyncList<GameObject> { }

    public class MatchMaker : NetworkBehaviour
    {
        public static MatchMaker instance;

        public SyncListMatch matches = new SyncListMatch();
        public SyncList<string> matchIDs = new SyncList<string>();
        public SyncList<string> usernames = new SyncList<string>();

        [SerializeField] private GameObject gameManagerPrefab;

        private void Start()
        {
            instance = this;
        }

        public bool HostGame(string matchID, GameObject player, bool publicMatch, out int playerIndex)
        {
            playerIndex = -1;

            if (!matchIDs.Contains(matchID))
            {
                matchIDs.Add(matchID);
                Match match = new Match(matchID, player);
                match.publicMatch = publicMatch;
                matches.Add(match);
                usernames.Add(player.GetComponent<LobbyPlayer>().username);

                playerIndex = 1;
                Debug.Log("Match Generated");
                return true;
            }
            else
            {
                Debug.Log("Match ID already exists");
                return false;
            }
        }

        public bool JoinGame(string matchID, GameObject player, out int playerIndex)
        {
            playerIndex = -1;
            if (matchIDs.Contains(matchID))
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    if (matches[i].matchID.Equals(matchID))
                    {
                        matches[i].players.Add(player);
                        usernames.Add(player.GetComponent<LobbyPlayer>().username);
                        playerIndex = matches[i].players.Count;
                        break;
                    }
                }

                Debug.Log("Match Joined");
                return true;
            }
            else
            {
                Debug.Log("Match ID doesn't exist");
                return false;
            }
        }

        public bool SearchGame(GameObject player, out int playerIndex, out string matchID)
        {
            playerIndex = -1;
            matchID = string.Empty;

            foreach (Match match in matches)
            {
                if (match.publicMatch && !match.matchFull && !match.inMatch)
                {
                    matchID = match.matchID;
                    if (JoinGame(matchID, player, out playerIndex))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void BeginGame(string matchID)
        {
            GameObject newGameManager = Instantiate(gameManagerPrefab);
            DontDestroyOnLoad(newGameManager);
            NetworkServer.Spawn(newGameManager);
            newGameManager.GetComponent<NetworkMatchChecker>().matchId = matchID.ToGuid();
            GameManager gameManager = newGameManager.GetComponent<GameManager>();

            foreach (Match match in matches)
            {
                if (match.matchID == matchID)
                {
                    foreach (GameObject player in match.players)
                    {
                        gameManager.AddPlayer(player);
                        LobbyPlayer lobbyPlayer = player.GetComponent<LobbyPlayer>();
                        lobbyPlayer.StartGame();
                    }

                    break;
                }
            }
        }

        public bool UsernameExists(string username)
        {
            foreach (string name in usernames)
            {
                if (name == username)
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetRandomMatchID()
        {
            string id = string.Empty;
            for (int i = 0; i < 5; i++)
            {
                int random = UnityEngine.Random.Range(0, 36);
                if (random < 26)
                {
                    id += (char)(random + 65);
                }
                else
                {
                    id += (random - 26).ToString();
                }
            }
            Debug.Log(id);
            return id;
        }

        public void PlayerDisconnected(LobbyPlayer player, string matchID)
        {
            usernames.Remove(player.username);

            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].matchID == matchID)
                {
                    int playerIndex = matches[i].players.IndexOf(player.gameObject);
                    matches[i].players.RemoveAt(playerIndex);
                    Debug.Log("Player disconnected from match " + matchID + " | " + matches[i].players.Count + " players remaining");

                    if (matches[i].players.Count == 0)
                    {
                        Debug.Log("No more players in Match. Terminating " + matchID);
                        matches.RemoveAt(i);
                        matchIDs.Remove(matchID);
                    }
                    else if (playerIndex == 0)
                    {
                        LobbyPlayer lobbyPlayer = matches[i].players[playerIndex].GetComponent<LobbyPlayer>();
                        Debug.Log("Host was disconnected from match. Transferring host to player '" + lobbyPlayer.username + "'");
                        lobbyPlayer.TargetBecameHost();
                    }
                    break;
                }
            }
        }

        public bool IsHost(LobbyPlayer player)
        {
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].matchID == player.matchID)
                {
                    return matches[i].players.IndexOf(player.gameObject) == 0;
                }
            }

            return false;
        }
    }

    public static class MatchExtensions
    {
        public static Guid ToGuid(this string id)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] inputBytes = Encoding.Default.GetBytes(id);
            byte[] hashBytes = provider.ComputeHash(inputBytes);

            return new Guid(hashBytes);
        }
    }
}