using UnityEngine;
using Mirror;

public class PlayerCollider : NetworkBehaviour
{
    [SerializeField] private PlayerMovementController player;
    [SerializeField] private ScoreController _score;

    [ServerCallback]
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (player.isDash)
        {
            if (hit.gameObject.TryGetComponent(out CollideStateController enemy))
            {
                if (!enemy.isImmune)
                {
                    enemy.ServerChangeColorOnCollision();
                    _score.ServerGivePoints(1);
                }
            }
        }
    }
}


