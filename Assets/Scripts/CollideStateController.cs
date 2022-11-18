using Mirror;
using System.Collections;
using UnityEngine;

public class CollideStateController : NetworkBehaviour
{
    [SerializeField] private Color _color;
    [SerializeField] private Color _collideColor;
    [SerializeField] private float _immuneTime;
    [SerializeField] private PlayerModelController _renderer;

    [SyncVar(hook = nameof(ClientSetColor))]
    private Color _currentColor;

    [SyncVar]
    private bool _isImmune;
    public bool isImmune => _isImmune;

    #region Client
    [Client]
    private void ClientSetColor(Color oldColor, Color newColor)
    {
        _renderer.ChangeColor(newColor);
    }
    #endregion

    #region Server
    [Server]
    public void ServerChangeColorOnCollision()
    {
        ServerSetImmune(true);
        ServerChangeColor(_collideColor);
        StartCoroutine(ServerColorTimer());
    }

    [Server]
    private void ServerChangeColor(Color color)
    {
        _currentColor = color;
    }

    [Server]
    private void ServerSetImmune(bool isImmune)
    {
        _isImmune = isImmune;
    }

    [Server]
    private IEnumerator ServerColorTimer()
    {
        yield return new WaitForSeconds(_immuneTime);
        ServerChangeColor(_color);
        ServerSetImmune(false);
    }
    #endregion


}
