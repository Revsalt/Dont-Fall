using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOverTimeLocaly : MonoBehaviour
{
    void OnEnable()
    {
        Destroy(gameObject, 2);
    }
}
