using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    [SyncVar] [SerializeField] private string username;
    [SyncVar] [SerializeField] private string initials;

    [Header("Health")]
    [SyncVar] public float maxHealth;

    [SyncVar] public float health;

    [Header("Speed & Dash")]
    public float speed;

    public float dashSpeed;
    public float maxDashTime;
    public float dashRecoverySpeed;

    [SyncVar] private bool isDashing;
    private float dashTime;

    [SyncVar] private int deaths;

    [Header("GameObjects")]
    [SerializeField] private SpriteRenderer circle;

    [SerializeField] private Transform healthBar;
    [SerializeField] private TextMeshPro usernameText;
    [SerializeField] private TextMeshPro deathsText;
    [SerializeField] private TextMeshPro initialsText;

    private Rigidbody2D rb;

    private Image healthBarGUI;
    private Image dashBarGUI;
    private TextMeshProUGUI deathsGUI;

    private Vector2 moveVelocity;

    private void Awake()
    {
        // Start() and Awake() are considered part of scene loading,
        // so we wait for the scene to load (all other start and awake functions
        // to run) before we run our "start" function
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 2)
        {
            deaths = 0;
            isDashing = false;
            dashTime = maxDashTime;

            // Get username and initials from LobbyPlayer and UIPlayer.ToInitials()
            MirrorNetworkConnect.LobbyPlayer lobbyPlayer = GetComponent<MirrorNetworkConnect.LobbyPlayer>();
            username = lobbyPlayer.username;
            initials = MirrorNetworkConnect.UIPlayer.ToInitials(lobbyPlayer.username);

            rb = GetComponent<Rigidbody2D>();

            if (isLocalPlayer)
            {
                Camera.main.GetComponent<CameraFollow>().target = transform;
                // Sadly, we have to get these at runtime, because you cannot assign gameobjects outside of prefabs
                // and these will only be visible if this is the local player.
                healthBarGUI = GameObject.Find("Canvas/Health Bar/Health").GetComponent<Image>();
                dashBarGUI = GameObject.Find("Canvas/Dash Bar/Dash").GetComponent<Image>();
                deathsGUI = GameObject.Find("Canvas/Deaths").GetComponent<TextMeshProUGUI>();
                GameObject.Find("Canvas/Username").GetComponent<TextMeshProUGUI>().SetText(username);

                transform.position = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1));
            }
            else
            {
                healthBar.parent.gameObject.SetActive(true); // Show health bar on other player's screens
                usernameText.gameObject.SetActive(true);
                usernameText.SetText(username);
                deathsText.gameObject.SetActive(true);
            }

            initialsText.SetText(initials);
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 2)
        {
            return;
        }

        if (isLocalPlayer)
        {
            // Rotate toward mouse
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 0;

            Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
            mousePos.x = mousePos.x - objectPos.x;
            mousePos.y = mousePos.y - objectPos.y;

            float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
            circle.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

            // Change move velocity
            Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            moveVelocity = moveInput * (isDashing ? dashSpeed : speed);

            // Change GUI health bar
            healthBarGUI.fillAmount = health / maxHealth;

            // Change GUI dash bar
            dashBarGUI.fillAmount = dashTime / maxDashTime;

            // Change GUI death count
            deathsGUI.SetText("<size=45><color=green>" + deaths + "</color></size> Deaths");

            // Dash
            if (Input.GetButtonDown("Dash") && !isDashing && dashTime == maxDashTime)
            {
                CmdSetDash(true);
            }

            if (dashTime == 0)
            {
                CmdSetDash(false);
            }

            if (isDashing) // degrade dashTime
            {
                dashTime = dashTime > 0 ? dashTime - Time.deltaTime : 0;
            }
            else // replenish dashTime
            {
                dashTime = dashTime < maxDashTime ? dashTime + Time.deltaTime * dashRecoverySpeed : maxDashTime;
            }
        }
        else
        {
            // Change health bar to reflect health
            Vector3 scale = healthBar.localScale;
            scale.x = health / maxHealth;
            healthBar.localScale = scale;

            // Change death count to reflect deaths
            deathsText.SetText("<size=3><color=green>" + deaths + "</color></size> Deaths");
        }
    }

    private void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().buildIndex != 2)
        {
            return;
        }

        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }

    [Command]
    private void CmdSetDash(bool dash)
    {
        isDashing = dash;
    }

    [ClientRpc]
    public void RpcDamagePlayer(float damage)
    {
        if (health - damage <= 0)
        {
            health = maxHealth;
            transform.position = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1));
            deaths++;
        }
        else
        {
            health -= damage;
        }
    }
}