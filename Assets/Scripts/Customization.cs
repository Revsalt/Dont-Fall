using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Customization : MonoBehaviour
{
    public static Customization Instance;

    public NetworkRoomManagerExt networkRoomManager;
    public InputField usernamefeild;
    public Slider EyeBrowThicknessFeild;
    public Slider EyeBrowCurvnessFeild;
    public Toggle EyeBrowVisible;
    public GameObject Model;
    public GameObject EyeModel;
    public GameObject[] EyeBrows;
    public GameObject[] EyeBrowsMeshs;
    public Image ImageColor;
    public Color[] ImageColors;
    public Mesh[] Models;
    public Mesh[] EyeModels;

    int i = 0;
    int x_ = 0;
    int i_ = 0;

    private void Start()
    {
        Instance = this;

        Invoke("setup", 0.2f);
    }

    void setup()
    {
        if (!PlayerSaving.PlayerDataExists())
        {
            PlayerData pd = new PlayerData()
            {
                Color = 0,
                ModelId = 0,
                EyeModelId = 0,
                EyeBrowParameters = Vector3.zero,
                username = "default"
            };

            PlayerSaving.Instance.AddPlayerDataFile(pd);
        }
        else
        {
            PlayerData pd = PlayerSaving.Instance.GetPlayerDataFile();
            SetColor(pd.Color);
            SetBody(pd.ModelId);
            SetEyes(pd.EyeModelId);
            SetEyeBrows(pd.EyeBrowParameters);
            usernamefeild.text = pd.username;

            EyeBrowThicknessFeild.value = pd.EyeBrowParameters.x;
            EyeBrowCurvnessFeild.value = pd.EyeBrowParameters.y;
            EyeBrowVisible.isOn = InttoBool(Mathf.RoundToInt(pd.EyeBrowParameters.z));

            UpdateEyebrowsPreview();
        }

        Done();
        gameObject.SetActive(false);
    }

    public Mesh GetEyeModel(int id)
    {
        return EyeModels[id];
    }

    public Mesh GetModel(int id)
    {
        return Models[id];
    }

    public Color GetColor(int id)
    {
        return ImageColors[id];
    }

    void SetColor(int o)
    {
        i = o;

        PressedColor(0);
    }

    public void SetEyeBrowsVisibility()
    {
        foreach (var item in EyeBrowsMeshs)
        {
            item.SetActive(EyeBrowVisible.isOn);
        }
    }

    public void PressedColor(int z)
    {
        if (i == ImageColors.Count() - 1)
            i = 0;
        else
            i = Mathf.Clamp(i + z, 0, ImageColors.Count() - 1);

        ImageColor.color = ImageColors[i];
        Model.GetComponent<SkinnedMeshRenderer>().material.color = ImageColor.color;
    }

    void SetEyes(int o)
    {
        x_ = o;
        ArrowsEyes(0);
    }

    void SetEyeBrows(Vector3 EyeBrowParameters)
    {
        foreach (var item in EyeBrows)
        {
            item.transform.localScale = new Vector3(1, 1, EyeBrowParameters.x);
            item.GetComponent<CuveEyeBrowModifier>().curvness = EyeBrowParameters.y;
        }
        foreach (var item in EyeBrowsMeshs)
        {
            item.SetActive(InttoBool(Mathf.RoundToInt(EyeBrowParameters.z)));
        }
    }

    public void UpdateEyebrowsPreview()
    {
        foreach (var item in EyeBrows)
        {
            item.transform.localScale = new Vector3(1, 1, EyeBrowThicknessFeild.value);
            item.GetComponent<CuveEyeBrowModifier>().curvness = EyeBrowCurvnessFeild.value;
            item.SetActive(EyeBrowVisible.isOn);
        }
    }

    public void ArrowsEyes(int z)
    {
        x_ = ClampedReset(x_ + z, 0, EyeModels.Count() - 1);

        EyeModel.GetComponent<SkinnedMeshRenderer>().sharedMesh = EyeModels[x_];
    }

    void SetBody(int o)
    { 
        i_ = o;
        ArrowsBody(0);
    }

    public void ArrowsBody(int z)
    {
        i_ = ClampedReset(i_+z , 0 ,Models.Count() - 1);

        Model.GetComponent<SkinnedMeshRenderer>().sharedMesh = Models[i_];
    }

    public void Done()
    {
        PlayerData pd = new PlayerData()
        {
            Color = i,
            ModelId = i_,
            EyeModelId = x_,
            EyeBrowParameters = new Vector3(EyeBrowThicknessFeild.value, EyeBrowCurvnessFeild.value, BooltoInt(EyeBrowVisible.isOn)),
            username = usernamefeild.text
        };

        PlayerSaving.Instance.UpdatePlayerDataFile(pd);

        networkRoomManager.SetPlayerColor(i);
        networkRoomManager.SetPlayerModel(i_);
        networkRoomManager.SetPlayerEyeModel(x_);
        networkRoomManager.SetPlayerEyeBrowParameters(new Vector3(EyeBrowThicknessFeild.value, EyeBrowCurvnessFeild.value, BooltoInt(EyeBrowVisible.isOn)));
        networkRoomManager.SetPlayerName(usernamefeild.text);
    }

    public static int ClampedReset(int value , int min , int max)
    {
        if (value > max)
            value = min;
        else if (value < min)
            value = max;

            return value;

    }

    public static int BooltoInt(bool b)
    {
        if (b)
            return 1;
        else return 0;
    }

    public static bool InttoBool(int b)
    {
        if (b == 1)
            return true;
        else return false;
    }
}
