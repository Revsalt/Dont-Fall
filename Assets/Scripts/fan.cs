using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fan : MonoBehaviour
{
    private void FixedUpdate()
    {
        transform.Rotate(0,0, 360 * Time.deltaTime);
    }
}
