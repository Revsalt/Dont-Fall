using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BombHandler : Item
{
    [Space]
    [SerializeField] MeshRenderer Bomb;
    [SerializeField] float explosionForce = 500f;
    [SerializeField] float Launchforce = 500f;
    [SerializeField] Material[] BombMaterials = new Material[2];
    [SerializeField][Range(0,1)]float RedFlickerSpeed = 1f;
    [SerializeField] ParticleSystem bombPrepareParticleSystem;

    public void Start()
    {
        StartCoroutine(Flicker());
    }

    public void Lauch()
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

    IEnumerator Flicker()
    {
        SetDefault();
        yield return new WaitForSeconds(RedFlickerSpeed + 0.2f);
        RedFlickerSpeed -= 0.2f;
        SetColors();
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(Flicker());
    }

    private void Update()
    {
        base.Update();

        bombPrepareParticleSystem.playbackSpeed = Mathf.Abs(RedFlickerSpeed) + 0;

        if (RedFlickerSpeed < 0.5f && !bombPrepareParticleSystem.isPlaying)
            bombPrepareParticleSystem.Play();
    }

    void SetDefault()
    {
        Bomb.GetComponent<Renderer>().material = BombMaterials[0];
    }

    public void Boom()
    {
        NetworkClient.connection.identity.GetComponent<Player>().ClientSideBoom(transform.position, explosionForce);
    }

    public void SetGameObjectParentToNull(Transform trans)
    {
        trans.SetParent(null);
    }

    void SetColors()
    {
        Bomb.GetComponent<Renderer>().material = BombMaterials[1];
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
