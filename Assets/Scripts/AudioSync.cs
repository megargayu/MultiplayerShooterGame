using Mirror;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSync : NetworkBehaviour
{
    public AudioSource source;
    public AudioClip[] clips;

    public void PlaySound(int id)
    {
        if (id >= 0 && id < clips.Length)
        {
            CmdPlaySound(id);
        }
    }

    [Command]
    private void CmdPlaySound(int id)
    {
        RpcPlaySound(id);
    }

    [ClientRpc]
    public void RpcPlaySound(int id)
    {
        if (id >= 0 && id < clips.Length)
        {
            source.PlayOneShot(clips[id]);
        }
    }
}