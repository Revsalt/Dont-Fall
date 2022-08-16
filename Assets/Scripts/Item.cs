using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using Mirror.Experimental;

public class Item : NetworkBehaviour
{
    public delegate void OnEnableInvicibilty();
    public static OnEnableInvicibilty onEnableInvicibilty;
    public delegate void OnDisableInvicibilty();
    public static OnDisableInvicibilty onDisableInvicibilty;

    [Header("DefaultProperties")]
    public float InvicibiltyTime = 1.5f;
    public bool followplayerRotation = false;
    [SyncVar] public bool isPicked;
    bool tracked;
    [HideInInspector] public bool canPick = true;

    public bool canInteract = true;
    [Header("CustomProperties")]
    public UnityEvent onInteract;

    public void Update()
    {
        if (isPicked)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().enabled = false;

            if (!followplayerRotation)
            {
                transform.rotation = Quaternion.FromToRotation(Vector3.up, Vector3.up);
            } else { transform.rotation = transform.parent.rotation; }
            tracked = true;

            GetComponent<NetworkRigidbody>().enabled = false;
            GetComponent<NetworkLerpRigidbody>().enabled = false;

            transform.localPosition = Vector3.zero;
        }
        else if (tracked)
        {
            StartCoroutine(Invicibility());
            GetComponent<Collider>().enabled = true;
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().AddForce(Vector3.up * 2, ForceMode.Impulse);
            tracked = false;

            GetComponent<NetworkRigidbody>().enabled = true;
            GetComponent<NetworkLerpRigidbody>().enabled = true;
        }
    }

    IEnumerator Invicibility()
    {
        canPick = false;
        if (onEnableInvicibilty != null)
            onEnableInvicibilty();
        yield return new WaitForSeconds(InvicibiltyTime);
        if (onDisableInvicibilty != null)
            onDisableInvicibilty();
        canPick = true;
    }

}
