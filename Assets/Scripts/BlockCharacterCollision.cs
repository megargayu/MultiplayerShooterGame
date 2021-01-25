using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCharacterCollision : MonoBehaviour
{
    [SerializeField] private CircleCollider2D characterCollider;
    [SerializeField] private CircleCollider2D characterBlockerCollider;

    private void Start()
    {
        Physics2D.IgnoreCollision(characterCollider, characterBlockerCollider, true);
    }
}