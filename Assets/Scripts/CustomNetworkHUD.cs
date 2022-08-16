using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class CustomNetworkHUD : NetworkManagerHUD
{
    public GameObject OfflinePanel, OnlinePanel;
    public static CustomNetworkHUD instance;

    private void Start()
    {
        instance = this;
    }

    public void InGame(bool b)
    {
        OfflinePanel.SetActive(!b);
        OnlinePanel.SetActive(b);
    }

    public void ResetCount()
    {
        oldcount = 0;
    }

    int oldcount = 0;
    string oldScene = "";
    private void Update()
    {
        if (oldScene != SceneManager.GetActiveScene().name)
        {
            oldScene = SceneManager.GetActiveScene().name;
            CustomNetworkHUD.instance.ResetCount();
        }

        if (NetworkRoomManagerExt.instance.roomSlots.Count != oldcount)
        {
            oldcount = NetworkRoomManagerExt.instance.roomSlots.Count;
            UpdatePlayersRooomListPreview();
        }

    }

    public void UpdatePlayersRooomListPreview()
    {
        StartCoroutine(UntillTrue());
    }

    IEnumerator UntillTrue()
    {
        yield return new WaitForSeconds(0.1f);

        if (LobbyContent.Instance == null)
        {
            Debug.Log("run");
            StartCoroutine(UntillTrue());
            yield break;
        }

        foreach (Transform item in LobbyContent.Instance.transform)
        {
            Destroy(item.gameObject);
        }

        List<NetworkRoomPlayer> roomPlayers = NetworkRoomManagerExt.instance.roomSlots;
        foreach (NetworkRoomPlayer roomPlayer in roomPlayers)
        {
            NetworkRoomPlayerPreview rp = Instantiate(LobbyContent.Instance.playerRoomPrefab, LobbyContent.Instance.transform).GetComponent<NetworkRoomPlayerPreview>();
            roomPlayer.GetComponent<NetworkRoomPlayerExt>().roomPlayerPreview = rp;
        }
    }
}
