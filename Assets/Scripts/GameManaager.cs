using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;
using Mirror.Discovery;

public class GameManaager : NetworkBehaviour 
{
    public static GameManaager instance;

    public Randrangeminandmax[] itemDelays;

    bool afterdisable = false;
    private void Start()
    {
        instance = this;
        enabled = false;
    }

    private void OnDisable()
    {
        afterdisable = true;
    }

    private void OnEnable()
    {
        if (afterdisable)
            BeginGame();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != NetworkRoomManagerExt.instance.GameplayScene)
        {
            StopAllCoroutines();
            enabled = false;
        }
    }

    public void BeginGame()
    {
        BegainSpawiningProps();
    }

    void BegainSpawiningProps()
    {
        if (NetworkServer.active)
        {
            for (int i = 0; i < NetworkRoomManagerExt.singleton.spawnPrefabs.Count; i++)
            {
                StartCoroutine(TickRate(i));
            }
        }
    }

    IEnumerator TickRate(int id)
    {
        yield return new WaitForSeconds(Random.Range(itemDelays[id].min, itemDelays[id].max));
        Spawn(id);
        StartCoroutine(TickRate(id));
    }

    internal static void Spawn(int id)
    {
        if (!NetworkServer.active) return;

        Vector3 spawnPosition = new Vector3(Random.Range(-5, 5), 10, Random.Range(-5, 5));
        NetworkServer.Spawn(Object.Instantiate(NetworkRoomManagerExt.singleton.spawnPrefabs[id], spawnPosition, Quaternion.identity));
    }


}

[System.Serializable]
public class Randrangeminandmax
{
    public int min = 0;
    public int max = 1;
}
