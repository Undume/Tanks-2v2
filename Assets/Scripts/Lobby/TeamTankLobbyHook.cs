using UnityEngine;
using UnityEngine.Networking;

public class TeamTankLobbyHook : Prototype.NetworkLobby.LobbyHook 
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        if (lobbyPlayer == null)
            return;

		TeamLobbyPlayer lp = lobbyPlayer.GetComponent<TeamLobbyPlayer>();

        if(lp != null)
            TeamGameManager.AddTank(gamePlayer, lp.team, lp.slot, lp.playerColor, lp.nameInput.text, lp.playerControllerId);
    }
}
