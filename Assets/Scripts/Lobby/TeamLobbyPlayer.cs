using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public enum Teams { None, Blue, Red }

public class TeamLobbyPlayer : NetworkLobbyPlayer
{
    // Team
    [SyncVar(hook = "OnTeamChanged")]
    public Teams team;
    [SyncVar(hook = "OnMyName")]
    public string playerName = "";
    [SyncVar(hook = "OnMyColor")]
    public Color playerColor = Color.white;

    public Button colorButton;
    public Button readyButton;
    public Button waitingPlayerButton;
    public Button joinATeamButton;
    public InputField nameInput;

    public Color MyColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
    public Color OtherColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

    static Color JoinColor = new Color(255.0f / 255.0f, 0.0f, 101.0f / 255.0f, 1.0f);
    static Color NotReadyColor = new Color(34.0f / 255.0f, 44 / 255.0f, 55.0f / 255.0f, 1.0f);
    static Color ReadyColor = new Color(0.0f, 204.0f / 255.0f, 204.0f / 255.0f, 1.0f);
    static Color TransparentColor = new Color(0, 0, 0, 0);

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();

        if (TeamLobbyManager.s_Singleton != null) TeamLobbyManager.s_Singleton.OnPlayersNumberModified(1);

        TeamLobbyPlayerList._instance.AddPlayer(this);
        TeamLobbyPlayerList._instance.DisplayDirectServerWarning(isServer && TeamLobbyManager.s_Singleton.matchMaker == null);

        if (isLocalPlayer)
        {
            SetupLocalPlayer();
        }
        else
        {
            SetupOtherPlayer();
        }

        //setup the player data on UI. The value are SyncVar so the player
        //will be created with the right value currently on server
        OnMyName(playerName);
        OnMyColor(playerColor);
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        //if we return from a game, color of text can still be the one for "Ready"
        readyButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;

        SetupLocalPlayer();
    }

    void ChangeReadyButtonColor(Color c)
    {
        ColorBlock b = readyButton.colors;
        b.normalColor = c;
        b.pressedColor = c;
        b.highlightedColor = c;
        b.disabledColor = c;
        readyButton.colors = b;
    }

    void SetupOtherPlayer()
    {
        joinATeamButton.GetComponent<Image>().color = OtherColor;
        nameInput.interactable = false;

        ChangeReadyButtonColor(NotReadyColor);

        readyButton.transform.GetChild(0).GetComponent<Text>().text = "...";
        readyButton.interactable = false;

        OnClientReady(false);
        GetComponent<Image>().color = OtherColor;
    }

    void SetupLocalPlayer()
    {
        joinATeamButton.GetComponent<Image>().color = MyColor;
        GetComponent<Image>().color = MyColor;

        nameInput.interactable = true;

        CheckRemoveButton();

        ChangeReadyButtonColor(JoinColor);

        joinATeamButton.gameObject.SetActive(true);

        readyButton.transform.GetChild(0).GetComponent<Text>().text = "JOIN";
        readyButton.interactable = true;

        //have to use child count of player prefab already setup as "this.slot" is not set yet
        if (playerName == "")
            CmdNameChanged("Player" + (TeamLobbyPlayerList._instance.noTeamPlayerListContentTransform.childCount - 1));
        nameInput.interactable = true;

        nameInput.onEndEdit.RemoveAllListeners();
        nameInput.onEndEdit.AddListener(OnNameChanged);

        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(OnReadyClicked);

        TeamLobbyPlayerList._instance.noneTeamButton.onClick.RemoveAllListeners();
        TeamLobbyPlayerList._instance.noneTeamButton.onClick.AddListener(OnClickNone);

        TeamLobbyPlayerList._instance.blueTeamButton.onClick.RemoveAllListeners();
        TeamLobbyPlayerList._instance.blueTeamButton.onClick.AddListener(OnClickBlue);

        TeamLobbyPlayerList._instance.redTeamButton.onClick.RemoveAllListeners();
        TeamLobbyPlayerList._instance.redTeamButton.onClick.AddListener(OnClickRed);

        //when OnClientEnterLobby is called, the loval PlayerController is not yet created, so we need to redo that here to disable
        //the add button if we reach maxLocalPlayer. We pass 0, as it was already counted on OnClientEnterLobby
        if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(0);


        TeamLobbyPlayerList._instance.RefreshLists();
    }

    //This enable/disable the remove button depending on if that is the only local player or not
    public void CheckRemoveButton()
    {
        if (!isLocalPlayer)
            return;

        int localPlayerCount = 0;
        foreach (PlayerController p in ClientScene.localPlayers)
            localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;
    }

    public override void OnClientReady(bool readyState)
    {
        if (readyState)
        {
            ChangeReadyButtonColor(TransparentColor);

            Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
            textComponent.text = "READY";
            textComponent.color = ReadyColor;
            readyButton.interactable = false;
            nameInput.interactable = false;
        }
        else
        {
            ChangeReadyButtonColor(isLocalPlayer ? JoinColor : NotReadyColor);

            Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
            textComponent.text = isLocalPlayer ? "JOIN" : "...";
            textComponent.color = Color.white;
            readyButton.interactable = isLocalPlayer;
            nameInput.interactable = isLocalPlayer;
        }
    }

    public void OnPlayerListChanged(int idx)
    {
    }

    ///===== callback from sync var

    public void OnMyName(string newName)
    {
        playerName = newName;
        nameInput.text = playerName;
    }

    public void OnMyColor(Color newColor)
    {
        playerColor = newColor;
        colorButton.GetComponent<Image>().color = newColor;
    }

    //===== UI Handler

    //Note that those handler use Command function, as we need to change the value on the server not locally
    //so that all client get the new value throught syncvar

    public void OnReadyClicked()
    {
        SendReadyToBeginMessage();
    }

    public void OnNameChanged(string str)
    {
        CmdNameChanged(str);
    }

    public void OnRemovePlayerClick()
    {
        if (isLocalPlayer)
        {
            RemovePlayer();
        }
        else if (isServer)
            LobbyManager.s_Singleton.KickPlayer(connectionToClient);

    }

    public void ToggleJoinButton(bool enabled)
    {
        readyButton.gameObject.SetActive(enabled);
        waitingPlayerButton.gameObject.SetActive(!enabled);
    }

    [ClientRpc]
    public void RpcUpdateCountdown(int countdown)
    {

    }

    [ClientRpc]
    public void RpcUpdateRemoveButton()
    {
        CheckRemoveButton();
    }


    [Command]
    public void CmdNameChanged(string name)
    {
        playerName = name;
    }

    [Command]
    public void CmdChangeColor(Color color)
    {
        playerColor = color;
    }

    //Cleanup thing when get destroy (which happen when client kick or disconnect)
    public void OnDestroy()
    {
        TeamLobbyPlayerList._instance.RemovePlayer(this);
        if (TeamLobbyManager.s_Singleton != null) TeamLobbyManager.s_Singleton.OnPlayersNumberModified(-1);
    }

    public void OnClickNone()
    {
        readyToBegin = false;
        SendNotReadyToBeginMessage();
        CmdSelectTeam(Teams.None);
    }

    public void OnClickRed()
    {

        if (TeamLobbyPlayerList._instance.redPlayerListContentTransform.childCount < 2)
        {
            CmdSelectTeam(Teams.Red);
            CmdChangeColor(Color.red);
        }
    }

    public void OnClickBlue()
    {
        if (TeamLobbyPlayerList._instance.bluePlayerListContentTransform.childCount < 2)
        {
            CmdSelectTeam(Teams.Blue);
            CmdChangeColor(new Color(0, .47f, 1f));
        }
    }

    [Command]
    public void CmdSelectTeam(Teams teamIndex)
    {
        // Set team of player on the server.
        team = teamIndex;
    }

    public void OnTeamChanged(Teams teamIndex)
    {
        team = teamIndex;
        joinATeamButton.gameObject.SetActive(teamIndex == Teams.None ? true : false);
        TeamLobbyPlayerList._instance.RefreshLists();
    }
}
