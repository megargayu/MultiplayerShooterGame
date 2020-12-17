using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Gun gun;
    public GameObject explosion;

    void Start()
    {
        Vector3 mouseDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        mouseDir.z = 0;
        mouseDir = mouseDir.normalized;

        transform.position += mouseDir;
        GetComponent<Rigidbody2D>().AddForce(mouseDir * gun.speed);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("hit " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("NetworkPlayer"))
        {
            Debug.Log("hit player");
            collision.gameObject.GetComponent<DummyPlayer>().health -= gun.damage;
        }

        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
