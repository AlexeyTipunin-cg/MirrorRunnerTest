using Mirror;
using System.Collections;
using UnityEngine;

public class ChangeColor : NetworkBehaviour
{
    [SerializeField] private Color _color;
    [SerializeField] private Color _collideColor;
    [SerializeField] private float _immuneTime;
    [SerializeField] private Renderer _renderer;

    [SyncVar(hook = nameof(ClientSetColor))]
    private Color _currentColor;

    [SyncVar]
    private bool _isImmune;

    private int _baseColorId = Shader.PropertyToID("_BaseColor");

    public bool isImmune => _isImmune;

    [Server]
    public void ChangeColorOnCollision()
    {
        ServerSetImmune(true);
        ServerChangeColor(_collideColor);
        StartCoroutine(ColorTimer());
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
    private IEnumerator ColorTimer()
    {
        yield return new WaitForSeconds(_immuneTime);
        ServerChangeColor(_color);
        ServerSetImmune(false);
    }

    [Client]
    private void ClientSetColor(Color oldColor, Color newColor)
    {
        _renderer.material.SetColor(_baseColorId, newColor);
    }
}
