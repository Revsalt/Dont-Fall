using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class GameControlsPanel : MonoBehaviour
{
    public static GameControlsPanel instace;
    public GameObject CreatingPlayerLoading;
    public Slider slider;
    public Text gameInteractionDisplay;
    public joyer touchJoystick;
    public Button jumpButton;
    public Button InteractButton;
    public GameObject[] UIelements;

    void Start()
    {
        instace = this;

        gameInteractionDisplay.text = "";
    }

    public void SetInteractButton(UnityEvent unityEvent)
    {
        InteractButton.onClick.RemoveAllListeners();
        InteractButton.onClick.AddListener(delegate { unityEvent.Invoke(); });
    }

    private void Update()
    {
        foreach (var item in UIelements)
        {
            float s = GameControlsPanel.instace.GetSliderFloat();
            item.transform.localScale = new Vector3(s, s, s);
        }
    }

    public float GetSliderFloat()
    {
        return slider.value;
    }
}
