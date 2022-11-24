using UnityEngine;
using Mirror;
using System.Collections;
using System.Linq;

public class PlayerMovementController : NetworkBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Transform _camera;

    [SerializeField] private float _movementSpeed = 50;
    [SerializeField] private float _lookSpeed = 1000;
    [SerializeField] private float _lookXLimit = 20;
    [SerializeField] private float _dashDistance;
    [SerializeField] private float _dashTime = 0.3f;

    [SerializeField] private PlayerModelController _changeModelColor;

    [SyncVar]
    private bool _isDash;
    private const float Y_POS = 0.5f;


    private Vector2 _rotation;
    public bool isDash => _isDash;

    public override void OnStartServer()
    {
        var currentPlayerRoom = (NetworkManager.singleton as LobbyManager).roomSlots.First(slot => slot.connectionToClient.connectionId == connectionToClient.connectionId) as RoomPlayer;
        connectionToClient.identity.name = currentPlayerRoom.playerName;
    }

    public override void OnStartLocalPlayer()
    {
        _camera.gameObject.SetActive(true);
    }

    #region Client

    [ClientCallback]
    private void Update()
    {
        if (ApplicationController.blockInput) return;

        if (!isLocalPlayer)
        {
            return;
        }

        if (_isDash)
        {
            float dashSpeed = _dashDistance / _dashTime;
            Vector3 dashMovement = transform.forward * dashSpeed * Time.deltaTime;
            CmdMoveCharacter(dashMovement);
            _changeModelColor.ClientRunAnimation(dashSpeed);
        }

        if (!_isDash)
        {
            float x = Input.GetAxis("Horizontal") * _movementSpeed * Time.deltaTime;
            float y = Input.GetAxis("Vertical") * _movementSpeed * Time.deltaTime;

            float rotY = Input.GetAxis("Mouse X") * _lookSpeed * Time.deltaTime;
            if (rotY != 0)
            {
                _rotation.y += Input.GetAxis("Mouse X") * _lookSpeed * Time.deltaTime;
                Vector3 rotation = new Vector3(0, _rotation.y, 0);
                CmdRotateCharacter(rotation);      
            }

            _rotation.x -= Input.GetAxis("Mouse Y") * _lookSpeed * Time.deltaTime;
            _rotation.x = Mathf.Clamp(_rotation.x, -_lookXLimit, _lookXLimit);

            Vector3 movement = (transform.right * x) + (transform.forward * y);

            if (Input.GetMouseButtonDown(0))
            {
                CmdLaunchTimer();
            }
            else if (movement == Vector3.zero)
            {
                _changeModelColor.ClientIdleAnimation();
            }
            else
            {
                CmdMoveCharacter(movement);
                _changeModelColor.ClientRunAnimation(_movementSpeed);
            }
        }

    }

    [ClientCallback]
    private void LateUpdate()
    {
        _camera.localRotation = Quaternion.Euler(_rotation.x, 0, 0);
    }

    #endregion

    #region Server

    [Command]
    private void CmdLaunchTimer()
    {
        _isDash = true;
        StartCoroutine(ServerDashTimer());
    }

    [Command]
    private void CmdMoveCharacter(Vector3 pos)
    {
        _characterController.Move(pos);
        transform.position = new Vector3(transform.position.x, Y_POS, transform.position.z);
    }

    [Command]
    private void CmdRotateCharacter(Vector3 rotation)
    {
        transform.eulerAngles = rotation;
    }

    [ServerCallback]
    private IEnumerator ServerDashTimer()
    {
        yield return new WaitForSeconds(_dashTime);
        _isDash = false;
    }
    #endregion
}
