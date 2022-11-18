using Mirror;


public class PlayerName : NetworkBehaviour
{
    private string _name;

    [Command]
    public void CommandSetName(string name)
    {
        _name = name;
    }
}
