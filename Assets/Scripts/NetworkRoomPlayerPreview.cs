using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class NetworkRoomPlayerPreview : MonoBehaviour
{
    public string username;
    public bool isReady;
    public Button removeButton;

    private void Update()
    {
        GetComponentInChildren<Text>().text = username;
    }
}
