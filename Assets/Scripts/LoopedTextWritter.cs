using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoopedTextWritter : MonoBehaviour
{
    public string[] texts;

    public TextMeshProUGUI _text;

    void OnEnable()
    {
        if (texts.Length > 0)
        {
            StartCoroutine(LoopedText()); 
        }
    }

    int textIndex = 0;
    IEnumerator LoopedText()
    {
        SetText(textIndex);
        textIndex++;
        yield return new WaitForSeconds(1);
        StartCoroutine(LoopedText());
    }

    void SetText(int index)
    {
        if (index > texts.Length - 1)
        {
            textIndex = 0;
            index = 0;
        }

        _text.text = texts[index];
    }
}
