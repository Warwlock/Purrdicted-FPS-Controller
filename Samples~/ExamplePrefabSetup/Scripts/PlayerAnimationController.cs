using UnityEngine;
using Warwlock.PlayerController;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeReference] private GameObject playerObject;
    private IMovement movement;
    private Animator anim;

    void Start()
    {
        movement = playerObject.GetComponent<IMovement>();
        anim = GetComponent<Animator>();
    }
    
    void Update()
    {
        if (movement == null || anim == null) return;

        anim.SetBool("isMoving", movement.MovementDirectionSpeed.magnitude > 0);
    }
}
