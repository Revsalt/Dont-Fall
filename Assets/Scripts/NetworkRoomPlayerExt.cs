using UnityEngine;
using UnityEngine.UI;
using Mirror;
using static NetworkRoomManagerExt;

public class NetworkRoomPlayerExt : NetworkRoomPlayer
{
    public NetworkRoomPlayerPreview roomPlayerPreview;
    [SyncVar] public string username;
    public int Color;
    public int Model;
    public int EyeModel;
    public Vector3 EyeBrowParameters;

    public override void OnStartClient()
    {
        // Debug.LogFormat(LogType.Log, "OnStartClient {0}", SceneManager.GetActiveScene().path);

        base.OnStartClient();
    }

    public override void OnClientEnterRoom()
    {
        // Debug.LogFormat(LogType.Log, "OnClientEnterRoom {0}", SceneManager.GetActiveScene().path);

    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        NetworkRoomManagerExt networkRoomManagerExt = NetworkRoomManagerExt.instance;
        NetworkClient.connection.Send(new CreatePlayerMessage
        {
            name = networkRoomManagerExt.PlayerName,
            color = networkRoomManagerExt.PlayerColor,
            eyeModel = networkRoomManagerExt.PlayerEyeModel,
            model = networkRoomManagerExt.PlayerModel,
            eyeBrowParameters = networkRoomManagerExt.PlayerEyeBrowParameters
        });
    }

    public override void OnClientExitRoom()
    {
        // Debug.LogFormat(LogType.Log, "OnClientExitRoom {0}", SceneManager.GetActiveScene().path);
    }

    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        // Debug.LogFormat(LogType.Log, "ReadyStateChanged {0}", newReadyState);
    }

    public void Update()
    {
        if (roomPlayerPreview)
            roomPlayerPreview.username = username;

        NetworkRoomManager room = NetworkManager.singleton as NetworkRoomManager;
        if (room)
        {
            if (!NetworkManager.IsSceneActive(room.RoomScene))
                return;

            if (roomPlayerPreview != null)
            {
                roomPlayerPreview.isReady = readyToBegin;

                DrawPlayerReadyState();
                DrawPlayerReadyButton();
            }
        }
    }

    void DrawPlayerReadyState()
    {
        roomPlayerPreview.removeButton.gameObject.SetActive(!isLocalPlayer && NetworkServer.active);
        roomPlayerPreview.removeButton.GetComponentInChildren<Text>().text = "REMOVE";
        roomPlayerPreview.removeButton.onClick.RemoveAllListeners();
        roomPlayerPreview.removeButton.onClick.AddListener(delegate { Remove(); });

        void Remove()
        {
            if (((isServer && index > 0) || isServerOnly))
            {
                // This button only shows on the Host for all players other than the Host
                // Host and Players can't remove themselves (stop the client instead)
                // Host can kick a Player this way.
                GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
            }
        }
    }

    void DrawPlayerReadyButton()
    {
        if (NetworkClient.active && isLocalPlayer)
        {
            LobbyContent.Instance.functionButtons[0].onClick.RemoveAllListeners();

            if (readyToBegin)
            {
                LobbyContent.Instance.functionButtons[0].GetComponentInChildren<Text>().text = "Cancel";
                LobbyContent.Instance.functionButtons[0].onClick.AddListener(delegate { CmdChangeReadyState(false); });
            }
            else
            {
                LobbyContent.Instance.functionButtons[0].GetComponentInChildren<Text>().text = "Ready";
                LobbyContent.Instance.functionButtons[0].onClick.AddListener(delegate { CmdChangeReadyState(true); });
                
            }
        }
    }
}

