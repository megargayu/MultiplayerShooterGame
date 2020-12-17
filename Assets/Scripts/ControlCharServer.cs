using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ControlCharServer : NetworkBehaviour
{
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangePosition(Vector3 pos)
    {
        rb.MovePosition(pos);
        ChangePositionServer(pos);
    }

    [Command]
    public void ChangePositionServer(Vector3 pos)
    {
        rb.MovePosition(pos);
    }
}
