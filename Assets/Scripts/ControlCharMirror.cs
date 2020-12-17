using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ControlCharMirror : NetworkBehaviour
{
    [SyncVar]
    public float health;
    public float speed;
    public Vector3 healthBarOffset;

    public Transform player;
    public Transform playerHealthBar;

    private Rigidbody2D rb;
    private Vector2 moveVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = player.GetComponent<Rigidbody2D>();

        if (isLocalPlayer)
        {
            Camera.main.GetComponent<CameraFollow>().target = player;
        } else
        {
            player.Find("Pointer").gameObject.SetActive(false); // Don't show pointers on other player's screens
            playerHealthBar.parent.gameObject.SetActive(true); // Show health bar on other player's screens
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            // Rotate toward mouse
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 0;

            Vector3 objectPos = Camera.main.WorldToScreenPoint(player.position);
            mousePos.x = mousePos.x - objectPos.x;
            mousePos.y = mousePos.y - objectPos.y;

            float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
            player.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

            // Change move velocity
            Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            moveVelocity = moveInput * speed;
        } else
        {
            // Change health bar to reflect health
            Vector3 scale = playerHealthBar.localScale;
            scale.x = health / 100f;
            playerHealthBar.localScale = scale;

            // Move health bar with player
            playerHealthBar.parent.localPosition = player.localPosition + healthBarOffset;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }   
}
