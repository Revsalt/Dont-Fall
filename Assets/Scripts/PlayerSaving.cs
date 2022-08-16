using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PlayerSaving : MonoBehaviour
{
    public static PlayerSaving Instance;
    void Start()
    {
        Instance = this;
    }

    public static bool PlayerDataExists()
    {
        return PlayerPrefs.HasKey("playerdata");
    }

    public void AddPlayerDataFile(PlayerData playerdata)
    {
        if (PlayerPrefs.HasKey("playerdata"))
            return;

        string json = JsonUtility.ToJson(playerdata);

        PlayerPrefs.SetString("playerdata", json);
    }

    public void UpdatePlayerDataFile(PlayerData newplayerdata)
    {
        if (PlayerPrefs.HasKey("playerdata"))
        {
            string json = JsonUtility.ToJson(newplayerdata);

            PlayerPrefs.SetString("playerdata", json);
        }

        return;
    }

    public PlayerData GetPlayerDataFile()
    {
        if (!PlayerPrefs.HasKey("playerdata"))
        {
            Debug.LogError("playerdata" + " not existent");
            return null;
        }

        string json = PlayerPrefs.GetString("playerdata");

        return JsonUtility.FromJson<PlayerData>(json);
    }
}
