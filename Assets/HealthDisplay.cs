using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    List<GameObject> hearts = new List<GameObject>();

    private void Start()
    {
        foreach (Transform item in transform)
        {
            hearts.Add(item.gameObject);
        }
    }

    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].SetActive(i < currentHealth);
        }
    }
}