using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    Material material;

    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * 2);

        material.color = new Color(material.color.r , material.color.g , material.color.b , Mathf.Clamp(Mathf.Sin(Time.time * 0.5f) / 2 , 0.2f , 1));
        float offset = Time.time * 1.3f;
        material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }
}
