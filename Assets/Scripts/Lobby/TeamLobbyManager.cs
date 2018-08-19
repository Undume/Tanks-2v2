using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Prototype.NetworkLobby;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;

public class TeamLobbyManager : LobbyManager
{

    static public TeamLobbyManager ts_Singleton;

    private void Awake()
    {
        ts_Singleton = this;
    }

    private bool IsMinimumPlayerInTeams()
    {
        bool inNone = TeamLobbyPlayerList._instance.noTeamPlayerListContentTransform.childCount == 0;
        bool inBlue = TeamLobbyPlayerList._instance.bluePlayerListContentTransform.childCount > 0;
        bool inRed = TeamLobbyPlayerList._instance.redPlayerListContentTransform.childCount > 0;
        return inNone && inBlue && inRed;
    }

    public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;

        TeamLobbyPlayer newPlayer = obj.GetComponent<TeamLobbyPlayer>();
        //newPlayer.ToggleJoinButton(numPlayers + 1 >= minPlayers);
        newPlayer.ToggleJoinButton(IsMinimumPlayerInTeams());


        for (int i = 0; i < lobbySlots.Length; ++i)
        {
            TeamLobbyPlayer p = lobbySlots[i] as TeamLobbyPlayer;

            if (p != null)
            {
                p.RpcUpdateRemoveButton();
                //p.ToggleJoinButton(numPlayers + 1 >= minPlayers %);
                p.ToggleJoinButton(IsMinimumPlayerInTeams());
            }
        }

        return obj;
    }

    public override void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId)
    {
        for (int i = 0; i < lobbySlots.Length; ++i)
        {
            TeamLobbyPlayer p = lobbySlots[i] as TeamLobbyPlayer;

            if (p != null)
            {
                p.RpcUpdateRemoveButton();
                //p.ToggleJoinButton(numPlayers + 1 >= minPlayers %);
                p.ToggleJoinButton(IsMinimumPlayerInTeams());
            }
        }
    }

    public override void OnLobbyServerDisconnect(NetworkConnection conn)
    {
        for (int i = 0; i < lobbySlots.Length; ++i)
        {
            TeamLobbyPlayer p = lobbySlots[i] as TeamLobbyPlayer;

            if (p != null)
            {
                p.RpcUpdateRemoveButton();
                //p.ToggleJoinButton(numPlayers + 1 >= minPlayers %);
                p.ToggleJoinButton(IsMinimumPlayerInTeams());
            }
        }
    }

    public void CheckJoinButton()
    {
        for (int i = 0; i < lobbySlots.Length; ++i)
        {
            TeamLobbyPlayer p = lobbySlots[i] as TeamLobbyPlayer;

            if (p != null)
            {
                p.RpcUpdateRemoveButton();
                //p.ToggleJoinButton(numPlayers + 1 >= minPlayers %);
                p.ToggleJoinButton(IsMinimumPlayerInTeams());
            }
        }
    }

    public override void OnLobbyServerPlayersReady()
    {
        bool allready = true;
        for (int i = 0; i < lobbySlots.Length; ++i)
        {
            if (lobbySlots[i] != null)
                allready &= lobbySlots[i].readyToBegin;
        }

        if (allready)
            ServerChangeScene(playScene);
    }
}
