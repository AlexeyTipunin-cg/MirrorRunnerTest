using Mirror;
using UnityEngine;

public class RoomPlayer : NetworkRoomPlayer
{
    [SyncVar]
    [SerializeField]
    private string _name;
    public string playerName => _name;

    [Command]
    public void CommandSetName(string name)
    {
        _name = name;
    }
}
