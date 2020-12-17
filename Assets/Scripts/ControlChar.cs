using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class ControlChar : MonoBehaviour
{
    public GameObject projectile;

    public Image healthBar;
    public Image dashBar;

    public float speed;
    public float dashSpeed;

    public float maxHealth;
    public float maxDashTime;

    public float dashRecoverySpeed;

    public Gun gun;
    public TextMeshProUGUI ammoLeft;

    private Rigidbody2D rb;
    private Vector2 moveVelocity;

    public float health;
    private float dashTime;
    private bool isDashing;

    private float timeSinceLastShot;

    // Start is called before the first frame update
    void Start()
    {
        // for testing only
        gun = new StartGun();
        ammoLeft.SetText(gun.ammoLeft + "/" + gun.ammoCapacity + " Ammo Remaining");

        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;
        dashTime = maxDashTime;
        isDashing = false;
        timeSinceLastShot = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate toward mouse
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;

        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

        // Character controller
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        moveVelocity = moveInput * ((isDashing ? dashSpeed : speed) - gun.weight);

        // Health controller
        healthBar.fillAmount = health / maxHealth;

        // Dash controller
        if (Input.GetButtonDown("Dash") && !isDashing && dashTime == maxDashTime)
        {
            isDashing = true;
        }
        
        if ((Input.GetButtonUp("Dash") || dashTime == 0) && isDashing)
        {
            isDashing = false;
        }

        if (isDashing) // degrade dashTime
        {
            dashTime = dashTime > 0 ? dashTime - Time.deltaTime : 0;
        } else // replenish dashTime
        {
            dashTime = dashTime < maxDashTime ? dashTime + Time.deltaTime * dashRecoverySpeed : maxDashTime;
        }

        dashBar.fillAmount = dashTime / maxDashTime;

        // Projectile controller
        if (Input.GetMouseButtonDown(0) || (Input.GetMouseButton(0) && timeSinceLastShot >= gun.reloadSpeed && gun.isMultishot))
        {
            if (gun.ammoLeft > 0)
            {
                // Shoot
                GameObject shot = Instantiate(projectile, transform.position, Quaternion.identity);
                shot.GetComponent<Projectile>().gun = gun;

                timeSinceLastShot = 0;
                gun.ammoLeft--;

                ammoLeft.SetText(gun.ammoLeft + "/" + gun.ammoCapacity + " Ammo Remaining");
            }
        } else
        {
            timeSinceLastShot += Time.deltaTime;
        }

        // Reload gun
        if (gun.ammoLeft == 0)
        {
            if (timeSinceLastShot >= gun.reloadSpeed)
            {
                Debug.Log("E");
                gun.ammoLeft = gun.ammoCapacity;
                ammoLeft.SetText(gun.ammoLeft + "/" + gun.ammoCapacity + " Ammo Remaining");
            }
            else
            {
                ammoLeft.SetText("Reloading...");
            }
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }
}
