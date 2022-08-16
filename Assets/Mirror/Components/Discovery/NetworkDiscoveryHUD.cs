using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Mirror;
using Mirror.Discovery;
using System.Collections;

[DisallowMultipleComponent]
[AddComponentMenu("Network/NetworkDiscoveryHUD")]
[HelpURL("https://mirror-networking.gitbook.io/docs/components/network-discovery")]
[RequireComponent(typeof(NetworkDiscovery))]
public class NetworkDiscoveryHUD : MonoBehaviour
{
    public UnityEvent OnConnectionNotFound;

    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();

    public NetworkDiscovery networkDiscovery;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (networkDiscovery == null)
        {
            networkDiscovery = GetComponent<NetworkDiscovery>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, OnDiscoveredServer);
            UnityEditor.Undo.RecordObjects(new Object[] { this, networkDiscovery }, "Set NetworkDiscovery");
        }
    }
#endif

    public void StartHost()
    {
        discoveredServers.Clear();
        NetworkManager.singleton.StartHost();
        networkDiscovery.AdvertiseServer();
    }

    public void StopHost()
    {
        NetworkManager.singleton.StopHost();
    }
    public void StopClient()
    {
        NetworkManager.singleton.StopClient();
    }

    float time;
    public void Connect()
    {
        discoveredServers.Clear();
        networkDiscovery.StartDiscovery();
        StartCoroutine(FindMatch());

        IEnumerator FindMatch()
        {
            for (time = 5; time > 0; time -= Time.deltaTime)
            {
                foreach (ServerResponse info in discoveredServers.Values)
                {
                    NetworkManager.singleton.StartClient(info.uri);
                    yield break;
                }
                yield return null;
            }

            OnConnectionNotFound.Invoke();
        }
    }

    public void OnDiscoveredServer(ServerResponse info)
    {
        // Note that you can check the versioning to decide if you can connect to the server or not using this method
        discoveredServers[info.serverId] = info;
    }
}
