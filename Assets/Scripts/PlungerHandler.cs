using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlungerHandler : Item
{
    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<Player>().SetParent(gameObject);
        }
    }
}
