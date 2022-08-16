using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TazerHandler : Item
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
