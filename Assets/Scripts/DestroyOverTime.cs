using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Mirror;
using Mirror.Discovery;


public class DestroyOverTime : NetworkBehaviour
{
    public float delay;
    public UnityEvent onDestroy;

    private void Start()
    {
        if (!isServer)
            return;

        StartCoroutine(Delay());
    }

    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Death"))
        {
            Die();
        }
    }

    public void Die()
    {
        if (GetComponent<NetworkIdentity>().hasAuthority)
            GetComponent<NetworkIdentity>().RemoveClientAuthority();

        RpcDie(gameObject);
        StartCoroutine(delay());

        IEnumerator delay()
        {
            yield return new WaitForSeconds(0.5f);
            NetworkServer.Destroy(gameObject);
        }
    }

    [ClientRpc]
    void RpcDie(GameObject g)
    {
        g.GetComponent<DestroyOverTime>().onDestroy.Invoke();
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(delay);
        Die();
    }
}
