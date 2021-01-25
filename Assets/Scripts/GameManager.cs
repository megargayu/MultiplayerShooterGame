using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MirrorNetworkConnect
{
    public class GameManager : NetworkBehaviour
    {
        private List<GameObject> players = new List<GameObject>();
        private GameObject enemy;
        private NetworkMatchChecker networkMatchChecker;

        private int currWave;
        private List<GameObject> enemies;
        private bool allLoaded = false;

        [ServerCallback]
        private void Start()
        {
            enemy = (GameObject)Resources.Load("Prefabs/Enemy");
            networkMatchChecker = GetComponent<NetworkMatchChecker>();
            enemies = new List<GameObject>();

            currWave = 1;
        }

        [ServerCallback]
        private void Update()
        {
            if (!allLoaded)
            {
                allLoaded = true;
                foreach (GameObject player in players)
                {
                    if (!player.GetComponent<LobbyPlayer>().sceneLoaded)
                    {
                        allLoaded = false;
                        return;
                    }
                }
            }

            bool allDestroyed = true;
            foreach (GameObject enemy in enemies)
            {
                if (enemy != null)
                {
                    allDestroyed = false;
                    break;
                }
            }

            if (allDestroyed)
            {
                currWave++;
                DoWave();
            }
        }

        private void DoWave()
        {
            for (int i = 0; i < currWave * 5; i++)
            {
                GameObject newEnemy = Instantiate(enemy, new Vector3(Random.Range(-20, 20), Random.Range(-20, 20)), Quaternion.identity);
                newEnemy.GetComponent<NetworkMatchChecker>().matchId = networkMatchChecker.matchId;
                enemies.Add(newEnemy);
                NetworkServer.Spawn(newEnemy);
            }
        }

        public void AddPlayer(GameObject player)
        {
            players.Add(player);
        }
    }
}