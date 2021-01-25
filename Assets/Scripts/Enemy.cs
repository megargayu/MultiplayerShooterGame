using UnityEngine;
using Mirror;

public class Enemy : NetworkBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] [SyncVar] private float health;
    [SerializeField] private float speed;
    [SerializeField] private float damage;

    [SerializeField] private GameObject explosion;
    [SerializeField] private SpriteRenderer circle;
    [SerializeField] private Transform healthBar;

    private GameObject nearestPlayer;
    private NetworkMatchChecker networkMatchChecker;

    private void Start()
    {
        health = maxHealth;
        networkMatchChecker = GetComponent<NetworkMatchChecker>();
    }

    private void Update()
    {
        if (isServer)
        {
            // Get nearest player
            nearestPlayer = null;
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (nearestPlayer == null) nearestPlayer = player;

                if (Vector3.Distance(transform.position, player.transform.position) < Vector3.Distance(nearestPlayer.transform.position, player.transform.position))
                {
                    nearestPlayer = player;
                }
            }

            // Rotate toward nearest player
            Vector3 playerPos = nearestPlayer.transform.position;
            playerPos.x = playerPos.x - transform.position.x;
            playerPos.y = playerPos.y - transform.position.y;
            playerPos.z = 0;

            float angle = Mathf.Atan2(playerPos.y, playerPos.x) * Mathf.Rad2Deg;
            circle.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        }
        else
        {
            // Change health bar to reflect health
            Vector3 scale = healthBar.localScale;
            scale.x = health / maxHealth;
            healthBar.localScale = scale;
        }
    }

    [ServerCallback]
    private void LateUpdate()
    {
        if (nearestPlayer != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, nearestPlayer.transform.position, speed * Time.fixedDeltaTime);
        }
    }

    [ServerCallback]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (LayerMask.LayerToName(collision.gameObject.layer) == "CharacterCollisionBlocker")
            {
                collision.gameObject.GetComponentInParent<PlayerController>().RpcDamagePlayer(damage);
            }
            else
            {
                collision.gameObject.GetComponent<PlayerController>().RpcDamagePlayer(damage);
            }

            GameObject explode = Instantiate(explosion, transform.position, Quaternion.identity);
            explode.GetComponent<NetworkMatchChecker>().matchId = networkMatchChecker.matchId;
            NetworkServer.Spawn(explode);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        if (health - damage <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            health -= damage;
        }
    }
}