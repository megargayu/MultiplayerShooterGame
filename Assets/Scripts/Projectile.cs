using Mirror;
using UnityEngine;

// TODO: If server is not in scene (so, in production, never), then projectiles do not detect walls and they never explode but keep going until infinity
public class Projectile : NetworkBehaviour
{
    [SyncVar] public float damage;
    public GameObject explosion;
    private NetworkMatchChecker networkMatchChecker;

    private void Start()
    {
        networkMatchChecker = GetComponent<NetworkMatchChecker>();
    }

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collider)
    {
        NetworkMatchChecker colMatchChecker = collider.GetComponent<NetworkMatchChecker>();
        if (collider.gameObject.CompareTag("Player") && colMatchChecker.matchId == networkMatchChecker.matchId)
        {
            collider.GetComponent<PlayerController>().RpcDamagePlayer(damage);
            collider.GetComponent<AudioSync>().RpcPlaySound(1);
        }
        else if (collider.gameObject.CompareTag("Enemy") && colMatchChecker.matchId == networkMatchChecker.matchId)
        {
            collider.GetComponent<Enemy>().TakeDamage(damage);
            collider.GetComponent<AudioSync>().RpcPlaySound(1);
        }

        if (colMatchChecker == null || collider.GetComponent<NetworkMatchChecker>().matchId == networkMatchChecker.matchId)
        {
            GameObject explode = Instantiate(explosion, transform.position, Quaternion.identity);
            explode.GetComponent<NetworkMatchChecker>().matchId = networkMatchChecker.matchId;
            NetworkServer.Spawn(explode);
            Destroy(gameObject);
        }
    }
}