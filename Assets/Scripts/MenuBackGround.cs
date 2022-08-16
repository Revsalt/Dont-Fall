using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBackGround : MonoBehaviour
{
    public GameObject foreground;
    [Header("IdleOptions")]
    public GameObject[] idleObjects;
    public float minSinLength, maxSinLength;
    public float minSinSpeed , maxSinSpeed;

    public float[] randomSeedSpeed;
    public float[] randomSeedLength;

    private void Start()
    {
        for (int i = 0; i < idleObjects.Length; i++)
        {
            float length = Random.Range(minSinLength, maxSinLength);
            float speed = Random.Range(minSinSpeed, maxSinSpeed);

            randomSeedSpeed[i] = speed;
            randomSeedLength[i] = length;
        }
    }

    void Update()
    {
        Color color = foreground.GetComponent<Image>().color;
        foreground.GetComponent<Image>().color = new Color(color.r , color.g , color.b , Mathf.Sin(Time.time * 2));

        for (int i = 0; i < idleObjects.Length; i++)
        {
            idleObjects[i].transform.localPosition = new Vector3(Mathf.Sin(Time.time * randomSeedSpeed[i] / 2) * randomSeedLength[i] / 2, Mathf.Sin(Time.time * randomSeedSpeed[i]) * randomSeedLength[i], 0);
            idleObjects[i].transform.localEulerAngles = new Vector3(0, 0, Mathf.Sin(Time.time * randomSeedSpeed[i] / 2) * randomSeedLength[i] / 2);
        }
    }
}
