using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAnimationHardCoded : MonoBehaviour
{
    [SerializeField] float timeSpeed = 4;
    [SerializeField] float lengthSpeed = 4;
    [SerializeField] Vector3 startpos;
    [SerializeField] Vector3 startrot;

    private void Start()
    {
        startpos = transform.position;
        startrot = transform.eulerAngles;
    }

    void Update()
    {
        transform.position = startpos + new Vector3(Mathf.Sin(Time.time * timeSpeed / 2) * lengthSpeed / 2, Mathf.Sin(Time.time * timeSpeed) * lengthSpeed, 0);
        transform.eulerAngles = startrot + new Vector3(0, 0, Mathf.Sin(Time.time * timeSpeed / 2) * lengthSpeed / 2);      
    }
}
