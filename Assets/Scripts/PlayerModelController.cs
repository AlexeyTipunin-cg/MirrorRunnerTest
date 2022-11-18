using Mirror;
using UnityEngine;

public class PlayerModelController : NetworkBehaviour
{
    private int _baseColorId = Shader.PropertyToID("_BaseColor");
    //private int _speedId = Animator.StringToHash("Speed");
    //private int _speedMulId = Animator.StringToHash("SpeedMultiplier");

    private const string _speedId = "Speed";
    private const string _speedMulId = "SpeedMultiplier";

    [SerializeField] private SkinnedMeshRenderer[] _renderers;
    [SerializeField] private Animator _animator;

    #region Client
    public void ChangeColor(Color newColor)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material.SetColor(_baseColorId, newColor);
        }
    }
    #endregion

    #region Server
    [Command]
    public void CmdRunAnimation(float movementSpeed)
    {
        _animator.SetFloat(_speedId, 1);
        _animator.SetFloat(_speedMulId, movementSpeed / 4);
    }

    [Command]
    public void CmdIdleAnimation()
    {
        _animator.SetFloat(_speedId, 0);
        _animator.SetFloat(_speedMulId, 1);
    }
    #endregion
}
