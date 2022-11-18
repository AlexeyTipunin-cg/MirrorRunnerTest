using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class LobbyManager : NetworkRoomManager
{
    private const string GAMEPLAY_SCENE = "BattleScene";
    public override void OnRoomServerAddPlayer(NetworkConnectionToClient conn)
    {
 
        base.OnRoomServerAddPlayer(conn);
        if (SceneManager.GetActiveScene().name == GAMEPLAY_SCENE)
        {
            var currentPlayerRoom = roomSlots.First(slot => slot.connectionToClient.connectionId == conn.connectionId) as RoomPlayer;
            conn.identity.name = currentPlayerRoom.playerName;
        }
    }
}
