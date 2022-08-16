using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;


public class NetworkRoomManagerExt : NetworkRoomManager
{
    public static NetworkRoomManagerExt instance;
    [HideInInspector] public bool isTelepathyTransport { get; set; } = true;

    [Header("Player Varaibles")]
    public string PlayerName;
    public int PlayerColor;
    public int PlayerModel;
    public int PlayerEyeModel;
    public Vector3 PlayerEyeBrowParameters;

    public void SetPlayerEyeModel(int meshid)
    {
        PlayerEyeModel = meshid;
    }

    public void SetPlayerName(string name)
    {
        PlayerName = name;
    }

    public void SetPlayerEyeBrowParameters(Vector3 eyeBrowParameters)
    {
        PlayerEyeBrowParameters = eyeBrowParameters;
    }

    public void SetPlayerModel(int meshid)
    {
        PlayerModel = meshid;
    }

    public void SetPlayerColor(int colorid)
    {
        PlayerColor = colorid;
    }

    public struct CreatePlayerMessage : NetworkMessage
    {
        public string name;
        public int color;
        public int model;
        public int eyeModel;
        public Vector3 eyeBrowParameters;
    }

    public void SetGameScene(string scene)
    {
        GameplayScene = scene;
    }

    public override void Awake()
    {
        base.Awake();
        instance = this;
    }

    private void Update()
    {
        if (isTelepathyTransport)
            transport = GetComponent<TelepathyTransport>();
        else
            transport = GetComponent<kcp2k.KcpTransport>();

        bool ingame = SceneManager.GetActiveScene().name == GameplayScene;
        GetComponent<CustomNetworkHUD>().InGame(ingame);

        if (ingame)
            GameManaager.instance.enabled = (true);
    }

    /// <summary>
    /// This is called on the server when a networked scene finishes loading.
    /// </summary>
    /// <param name="sceneName">Name of the new scene.</param>
    public override void OnRoomServerSceneChanged(string sceneName)
    {
        // spawn the initial batch of Rewards aka props
        //if (sceneName == GameplayScene)
        //{
        //    Spawner.InitialSpawn();
        //}
    }

    /// <summary>
    /// Called just after GamePlayer object is instantiated and just before it replaces RoomPlayer object.
    /// This is the ideal point to pass any data like player name, credentials, tokens, colors, etc.
    /// into the GamePlayer object as it is about to enter the Online scene.
    /// </summary>
    /// <param name="roomPlayer"></param>
    /// <param name="gamePlayer"></param>
    /// <returns>true unless some code in here decides it needs to abort the replacement</returns>
    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnection conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        Player player = gamePlayer.GetComponent<Player>();
        NetworkRoomPlayerExt _roomplayer = roomPlayer.GetComponent<NetworkRoomPlayerExt>();
        player.playerName = _roomplayer.username;
        player.playerColor = _roomplayer.Color;
        player.playerModel = _roomplayer.Model;
        player.playerEyeModel = _roomplayer.EyeModel;
        player.playerEyeBrowParameters = _roomplayer.EyeBrowParameters;

        return true;
    }

    public void ReturntoLobby()
    {
        ServerChangeScene(RoomScene);
    }

    public override void OnRoomStopClient()
    {
        base.OnRoomStopClient();
    }

    public override void OnRoomStopServer()
    {
        base.OnRoomStopServer();
    }

    /*
        This code below is to demonstrate how to do a Start button that only appears for the Host player
        showStartButton is a local bool that's needed because OnRoomServerPlayersReady is only fired when
        all players are ready, but if a player cancels their ready state there's no callback to set it back to false
        Therefore, allPlayersReady is used in combination with showStartButton to show/hide the Start button correctly.
        Setting showStartButton false when the button is pressed hides it in the game scene since NetworkRoomManager
        is set as DontDestroyOnLoad = true.
    */

    bool showStartButton;

    public override void OnRoomServerPlayersReady()
    {
        // calling the base method calls ServerChangeScene as soon as all players are in Ready state.
#if UNITY_SERVER
            base.OnRoomServerPlayersReady();
#else
        showStartButton = true;
#endif
    }

    public override void OnGUI()
    {
        base.OnGUI();

        if (!LobbyContent.Instance || !SceneManagerHandler.instance)
            return;

        if (!NetworkServer.active)
            SceneManagerHandler.instance.DisableInteraction();

        LobbyContent.Instance.functionButtons[1].gameObject.SetActive(true);

        LobbyContent.Instance.functionButtons[1].onClick.RemoveAllListeners();

        if (allPlayersReady)
        {
            LobbyContent.Instance.functionButtons[1].GetComponentInChildren<Text>().text = "STARTGAME";
            LobbyContent.Instance.functionButtons[1].onClick.AddListener(delegate { startGame(); });
        }
        else
        {
            LobbyContent.Instance.functionButtons[1].GetComponentInChildren<Text>().text = "LEAVE";
            LobbyContent.Instance.functionButtons[1].onClick.AddListener(delegate { leaveGame(); });
        }

        void startGame()
        {
            if (allPlayersReady && showStartButton)
            {
                // set to false to hide it in the game scene
                showStartButton = false;

                ServerChangeScene(GameplayScene);
            }
        }
    }

    public void leaveGame()
    {
        if (NetworkServer.active)
            StopHost();
        else
            StopClient();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<CreatePlayerMessage>(OnCreatePlayer);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("client");

        base.OnClientConnect(conn);
    }

    void OnCreatePlayer(NetworkConnection connection, CreatePlayerMessage createPlayerMessage)
    {
        if (connection.identity != null)
        {
            connection.identity.GetComponent<NetworkRoomPlayerExt>().username = createPlayerMessage.name;
            connection.identity.GetComponent<NetworkRoomPlayerExt>().Color = createPlayerMessage.color;
            connection.identity.GetComponent<NetworkRoomPlayerExt>().Model = createPlayerMessage.model;
            connection.identity.GetComponent<NetworkRoomPlayerExt>().EyeModel = createPlayerMessage.eyeModel;
            connection.identity.GetComponent<NetworkRoomPlayerExt>().EyeBrowParameters = createPlayerMessage.eyeBrowParameters;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        // increment the index before adding the player, so first player starts at 1
        clientIndex++;

        if (IsSceneActive(RoomScene))
        {
            if (roomSlots.Count == maxConnections)
                return;

            allPlayersReady = false;

            // Debug.LogFormat(LogType.Log, "NetworkRoomManager.OnServerAddPlayer playerPrefab:{0}", roomPlayerPrefab.name);

            GameObject newRoomGameObject = OnRoomServerCreateRoomPlayer(conn);
            if (newRoomGameObject == null)
            {
                newRoomGameObject = Instantiate(roomPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);
            }

            //newRoomGameObject.GetComponent<NetworkRoomPlayerExt>().playerMessage = pm;

            NetworkServer.AddPlayerForConnection(conn, newRoomGameObject);
        }
        else
            OnRoomServerAddPlayer(conn);

    }
}

[System.Serializable]
public class PlayerMessage
{
    public string PlayerName = "";
    public Vector3 PlayerColor = Vector3.zero;
    public int PlayerEyeModel = 0;
    public int PlayerModel = 0;
}
