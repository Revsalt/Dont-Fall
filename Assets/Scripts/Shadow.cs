using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    public LayerMask modelCastLayerMask;
    float startscale;

    private void Start()
    {
        startscale = transform.localScale.x;
        GetComponent<MeshRenderer>().enabled = true;
    }

    private void OnEnable()
    {
        GetComponent<MeshRenderer>().enabled = true;
    }

    void Update()
    {
        RaycastHit hit;
        Physics.Raycast(transform.parent.position, -Vector3.up, out hit, 100, modelCastLayerMask);

        transform.position = hit.point + Vector3.up * 0.05f;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        float s = Mathf.InverseLerp(0 , 1 , startscale - Vector3.Distance(hit.point, transform.parent.position) / 20);
        transform.localScale = new Vector3(s, 0, s);
    }

    private void OnDisable()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
}
