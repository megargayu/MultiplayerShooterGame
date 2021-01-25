using Mirror;
using UnityEngine;

[RequireComponent(typeof(AudioSync))]
[RequireComponent(typeof(NetworkMatchChecker))]
public class ShootProjectile : NetworkBehaviour
{
    public GameObject projectile;
    public float projectileSpeed;

    [SerializeField] private AudioSync audioSync;
    [SerializeField] private NetworkMatchChecker networkMatchChecker;

    private void Update()
    {
        if (isLocalPlayer && Input.GetMouseButtonDown(0))
        {
            Vector3 mouseDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            mouseDir.z = 0;
            mouseDir = mouseDir.normalized;

            audioSync.PlaySound(0);
            CmdShoot(mouseDir);
        }
    }

    [Command]
    private void CmdShoot(Vector3 mouseDir)
    {
        GameObject shot = Instantiate(projectile, transform.position, Quaternion.identity);
        shot.transform.position += mouseDir;
        shot.GetComponent<NetworkMatchChecker>().matchId = networkMatchChecker.matchId;
        shot.GetComponent<Rigidbody2D>().velocity = mouseDir * projectileSpeed;
        NetworkServer.Spawn(shot);
    }
}