using Mirror;
using UnityEngine;

public class CameraRotation : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float _lookSpeed;
    [SerializeField] private float _lookXLimit;
    [SerializeField] private Transform _camera;


    public override void OnStartLocalPlayer()
    {
        _camera.gameObject.SetActive(true);
    }

    private Vector2 _rotation;
    private void LateUpdate()
    {
        if (ApplicationController.blockInput) return;

        _rotation.y += Input.GetAxis("Mouse X") * _lookSpeed * Time.deltaTime;
        _rotation.x -= Input.GetAxis("Mouse Y") * _lookSpeed * Time.deltaTime;
        _rotation.x = Mathf.Clamp(_rotation.x, -_lookXLimit, _lookXLimit);
        _camera.localRotation = Quaternion.Euler(_rotation.x, 0, 0);
        transform.eulerAngles = new Vector2(0, _rotation.y);
    }
}
