using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using Prototype.NetworkLobby;

public class LobbyMenuTeams : MonoBehaviour
{

    public TeamLobbyManager lobbyManager;
    public RectTransform lobbyPanel;

    public Text headerLanIP;
    public Text hostAndPlayIP;

    public InputField ipInput;

    public void OnEnable()
    {
        lobbyManager.topPanel.ToggleVisibility(true);
        hostAndPlayIP.text = "Local IP: " + LocalIPAddress();

        ipInput.onEndEdit.RemoveAllListeners();
        ipInput.onEndEdit.AddListener(onEndEditIP);
    }

    public void OnClickHost()
    {
        lobbyManager.StartHost();
        headerLanIP.text = "Local IP: " + LocalIPAddress();
    }

    public void OnClickJoin()
    {
        lobbyManager.ChangeTo(lobbyPanel);
        lobbyManager.networkAddress = ipInput.text;
        headerLanIP.text = ipInput.text;
        lobbyManager.StartClient();

        lobbyManager.backDelegate = lobbyManager.StopClientClbk;
        lobbyManager.DisplayIsConnecting();

        lobbyManager.SetServerInfo("Connecting...", lobbyManager.networkAddress);
    }

    public string LocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }

    void onEndEditIP(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnClickJoin();
        }
    }
}
