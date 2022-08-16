using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyContent : MonoBehaviour
{
    public static LobbyContent Instance;
    public GameObject playerRoomPrefab;

    public Button[] functionButtons;

    private void Start()
    {
        Instance = this;
    }
}
