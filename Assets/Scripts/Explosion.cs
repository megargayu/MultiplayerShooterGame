using Mirror;
using UnityEngine;

[RequireComponent(typeof(AudioSync))]
public class Explosion : NetworkBehaviour
{
    [ServerCallback]
    private void Start()
    {
        GetComponent<AudioSync>().RpcPlaySound(0);
        Destroy(gameObject, GetComponent<ParticleSystem>().main.startLifetime.constantMax);
    }
}