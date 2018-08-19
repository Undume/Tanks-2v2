using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

class TeamLobbyPlayerList : MonoBehaviour
{
    public static TeamLobbyPlayerList _instance = null;

    public RectTransform noTeamPlayerListContentTransform;
    public RectTransform bluePlayerListContentTransform;
    public RectTransform redPlayerListContentTransform;
    public Text nPlayersBlue;
    public Text nPlayersRed;
    public GameObject warningDirectPlayServer;
    public Button noneTeamButton;
    public Button redTeamButton;
    public Button blueTeamButton;

    protected VerticalLayoutGroup _noLayout;
    protected VerticalLayoutGroup _redLayout;
    protected VerticalLayoutGroup _blueLayout;
    public List<TeamLobbyPlayer> players = new List<TeamLobbyPlayer>();

    public void OnEnable()
    {
        _instance = this;
        _noLayout = noTeamPlayerListContentTransform.GetComponent<VerticalLayoutGroup>();
        _blueLayout = bluePlayerListContentTransform.GetComponent<VerticalLayoutGroup>();
        _redLayout = redPlayerListContentTransform.GetComponent<VerticalLayoutGroup>();
    }

    public void DisplayDirectServerWarning(bool enabled)
    {
        if (warningDirectPlayServer != null)
            warningDirectPlayServer.SetActive(enabled);
    }

    void Update()
    {
        //this dirty the layout to force it to recompute evryframe (a sync problem between client/server
        //sometime to child being assigned before layout was enabled/init, leading to broken layouting)

        if (_noLayout)
            _noLayout.childAlignment = Time.frameCount % 2 == 0 ? TextAnchor.UpperCenter : TextAnchor.UpperLeft;
        if (_blueLayout)
            _blueLayout.childAlignment = Time.frameCount % 2 == 0 ? TextAnchor.UpperCenter : TextAnchor.UpperLeft;
        if (_redLayout)
            _redLayout.childAlignment = Time.frameCount % 2 == 0 ? TextAnchor.UpperCenter : TextAnchor.UpperLeft;
    }

    public void MovePlayer(TeamLobbyPlayer player)
    {
        switch (player.team)
        {
            case Teams.None:
                player.transform.SetParent(noTeamPlayerListContentTransform, false);
                break;
            case Teams.Blue:
                player.transform.SetParent(bluePlayerListContentTransform, false);
                break;
            case Teams.Red:
                player.transform.SetParent(redPlayerListContentTransform, false);
                break;
        }
    }

    public void AddPlayer(TeamLobbyPlayer player)
    {
        if (players.Contains(player))
            return;

        players.Add(player);
        player.transform.SetParent(noTeamPlayerListContentTransform, false);
        PlayerListModified();
    }

    public void RemovePlayer(TeamLobbyPlayer player)
    {
        players.Remove(player);
        PlayerListModified();
    }

    public void PlayerListModified()
    {
        int i = 0;
        foreach (TeamLobbyPlayer p in players)
        {
            p.OnPlayerListChanged(i);
            ++i;
        }
    }

    private void CountingTeams()
    {
        nPlayersBlue.text = bluePlayerListContentTransform.childCount + "/2 players";
        nPlayersRed.text = redPlayerListContentTransform.childCount + "/2 players";
    }

    public void RefreshLists()
    {
        foreach (TeamLobbyPlayer p in players)
        {
            MovePlayer(p);
        }
        PlayerListModified();
        CountingTeams();
        TeamLobbyManager.ts_Singleton.CheckJoinButton();
    }
}


