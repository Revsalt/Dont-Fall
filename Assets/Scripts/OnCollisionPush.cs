using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OnCollisionPush : MonoBehaviour
{
    public BoxingGlovesHaandler bgh;

    [ServerCallback]
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && collision.gameObject)
            bgh.KnockBack(collision.GetComponent<Player>());
    }
}
