using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class SceneManagerHandler : NetworkBehaviour
{
    public static SceneManagerHandler instance;

    public Sprite[] SceneImages;
    [Header("UI")]
    public Image image;
    public GameObject[] arrows;

    [SyncVar]int i = 0;
    private void Start()
    {
        instance = this;
        SetSceneImage(0);
    }

    private void Update()
    {
        int sceneid = i;
        SetSceneImage(sceneid);
    }

    public void DisableInteraction()
    {
        foreach (var item in arrows)
        {
            item.SetActive(false);
        }
    }

    public void ArrowsScene(int z)
    {
        i = Customization.ClampedReset(i + z, 0, SceneImages.Count() - 1);
    }

    public void SetSceneImage(int i)
    {
        image.sprite = SceneImages[i];
        NetworkRoomManagerExt.instance.SetGameScene(NameFromIndex(i + 2));
    }

    private static string NameFromIndex(int BuildIndex)
    {
        string path = SceneUtility.GetScenePathByBuildIndex(BuildIndex);
        int slash = path.LastIndexOf('/');
        string name = path.Substring(slash + 1);
        int dot = name.LastIndexOf('.');
        return name.Substring(0, dot);
    }
}
