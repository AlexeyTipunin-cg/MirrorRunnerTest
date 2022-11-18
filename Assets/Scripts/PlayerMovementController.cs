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

    #region Start & Stop Callbacks

    /// <summary>
    /// This is invoked for NetworkBehaviour objects when they become active on the server.
    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
    /// </summary>
    public override void OnStartServer()
    {
        var currentPlayerRoom = (NetworkManager.singleton as LobbyManager).roomSlots.First(slot => slot.connectionToClient.connectionId == connectionToClient.connectionId) as RoomPlayer;
        connectionToClient.identity.name = currentPlayerRoom.playerName;
    }

    /// <summary>
    /// Invoked on the server when the object is unspawned
    /// <para>Useful for saving object data in persistent storage</para>
    /// </summary>
    public override void OnStopServer() { }

    /// <summary>
    /// Called on every NetworkBehaviour when it is activated on a client.
    /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
    /// </summary>
    public override void OnStartClient() { }

    /// <summary>
    /// This is invoked on clients when the server has caused this object to be destroyed.
    /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
    /// </summary>
    public override void OnStopClient() { }

    /// <summary>
    /// Called when the local player object has been set up.
    /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        _camera.gameObject.SetActive(true);
    }

    /// <summary>
    /// Called when the local player object is being stopped.
    /// <para>This happens before OnStopClient(), as it may be triggered by an ownership message from the server, or because the player object is being destroyed. This is an appropriate place to deactivate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStopLocalPlayer() { }

    /// <summary>
    /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
    /// <para>This is called after <see cref="OnStartServer">OnStartServer</see> and before <see cref="OnStartClient">OnStartClient.</see></para>
    /// <para>When <see cref="NetworkIdentity.AssignClientAuthority">AssignClientAuthority</see> is called on the server, this will be called on the client that owns the object. When an object is spawned with <see cref="NetworkServer.Spawn">NetworkServer.Spawn</see> with a NetworkConnectionToClient parameter included, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStartAuthority() { }

    /// <summary>
    /// This is invoked on behaviours when authority is removed.
    /// <para>When NetworkIdentity.RemoveClientAuthority is called on the server, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStopAuthority() { }


    #endregion

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
