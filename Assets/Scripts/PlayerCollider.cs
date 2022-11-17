using UnityEngine;
using Mirror;

public class PlayerCollider : NetworkBehaviour
{
    [SerializeField] private NetworkPlayer player;

    [ServerCallback]
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (player.isDash)
        {
            if (hit.gameObject.TryGetComponent(out ChangeColor enemy))
            {
                if (!enemy.isImmune)
                {
                    enemy.ChangeColorOnCollision();
                }
            }
        }
    }
}


