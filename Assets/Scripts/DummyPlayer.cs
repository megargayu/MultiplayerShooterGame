using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayer : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health;

    public Transform healthContainer;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 scale = healthContainer.localScale;
        scale.x = health / maxHealth;
        healthContainer.localScale = scale;
    }
}
