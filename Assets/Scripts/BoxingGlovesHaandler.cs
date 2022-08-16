using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BoxingGlovesHaandler : Item
{
    public GameObject BoxingGlovesFist,BoxGloveSpring,BoxingGlovesFistSocket;
    public float GloveDistance = 0;

    float startscale = 0;
    private void Start()
    {
        startscale = BoxGloveSpring.transform.localScale.x;
    }

    private void Update()
    {
        base.Update();

        BoxGloveSpring.transform.localScale = Vector3.Lerp(BoxGloveSpring.transform.localScale , new Vector3(startscale, GloveDistance , startscale) , 7 * Time.deltaTime);

        BoxingGlovesFist.transform.position = BoxingGlovesFistSocket.transform.position;
    }

    public void Punch()
    {
        CmdPunch(gameObject);
    }

    [Command(requiresAuthority = false)]
    public void CmdPunch(GameObject g)
    {
        g.GetComponent<Animator>().SetTrigger("isPunch");

        RpcPunch(g);
    }

    [ClientRpc]
    public void RpcPunch(GameObject g)
    {
        if (NetworkServer.active)
            return;

        g.GetComponent<Animator>().SetTrigger("isPunch");
    }

    public void KnockBack(Player player)
    {
        if (isPicked)
            RpcKnockBack(player.GetComponent<NetworkIdentity>().connectionToClient);
    }

    [TargetRpc]
    public void RpcKnockBack(NetworkConnection conn)
    {
        NetworkClient.connection.identity.GetComponent<Player>().KnockBack(30, transform.right);
    }

    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<Player>().SetParent(gameObject);
        }
    }
}
