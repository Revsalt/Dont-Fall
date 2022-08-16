using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuveEyeBrowModifier : MonoBehaviour
{
    public float curvness;

    public GameObject[] Bones;
    List<Vector3> startRotationBones = new List<Vector3>();

    private void Start()
    {
        foreach (var item in Bones)
        {
            startRotationBones.Add(item.transform.localEulerAngles);
        }
    }

    float oldCurve = 0;
    private void FixedUpdate()
    {
        if (curvness == oldCurve)
        {
            return;
        }else { oldCurve = curvness; }

        Bones[0].transform.localEulerAngles = startRotationBones[0] + new Vector3(0, curvness / 2, 0);
        Bones[1].transform.localEulerAngles = startRotationBones[1] + new Vector3(curvness, 0, 0);
        Bones[2].transform.localEulerAngles = startRotationBones[2] + new Vector3(0, -curvness / 2, 0);
        Bones[3].transform.localEulerAngles = startRotationBones[3] + new Vector3(curvness, 0, 0);
    }
}
