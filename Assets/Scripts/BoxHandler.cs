using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using EZCameraShake;

public class BoxHandler : Item
{
    public float Launchforce = 500f;
    public void LauchBox()
    {
        NetworkClient.connection.identity.GetComponent<Player>().DropItem(gameObject);
        CmdLaunch(gameObject);
    }

    [Command(requiresAuthority = false)]
    public void CmdLaunch(GameObject _object)
    {
        _object.GetComponent<Rigidbody>().isKinematic = false;
        _object.GetComponent<Rigidbody>().AddForce(transform.right * Launchforce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (CameraShaker.Instance != null)
            CameraShaker.Instance.ShakeOnce(3, 1, 0.3f, 0.2f);

        if (!isServer)
            return;

        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<Player>().SetParent(gameObject);
        }
    }
}
