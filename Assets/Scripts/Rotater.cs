using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    [SerializeField]float speed = -90;
    void Update()
    {
        transform.Rotate(0, speed * Time.deltaTime, 0);   
    }
}
