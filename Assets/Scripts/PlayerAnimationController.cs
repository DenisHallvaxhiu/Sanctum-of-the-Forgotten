using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private PlayerEvents playerEvents;
    private PlayerAttack playerAttack;
    private Animator animator;

    // Animator Params
    const string MOVE_STRAIGHT = "isMovingStraight";
    const string RUN = "isRunning";
    const string JUMP = "isJumping";
    const string CROUCH = "isCrouching";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerEvents = GetComponentInParent<PlayerEvents>();
        playerAttack = GetComponentInParent<PlayerAttack>();
    }

    private void OnEnable()
    {
        if (playerEvents == null) return;

        playerEvents.Move += PlayerEvents_Move;
        playerEvents.Sprint += PlayerEvents_Sprint;
        playerEvents.Jump += PlayerEvents_Jump;
        playerEvents.Crouch += PlayerEvents_Crouch;
    }

    private void OnDisable()
    {
        if (playerEvents == null) return;

        playerEvents.Move -= PlayerEvents_Move;
        playerEvents.Sprint -= PlayerEvents_Sprint;
        playerEvents.Jump -= PlayerEvents_Jump;
        playerEvents.Crouch -= PlayerEvents_Crouch;
    }

    private void PlayerEvents_Move(bool isMoving)
    {
        animator.SetBool(MOVE_STRAIGHT, isMoving);
    }

    private void PlayerEvents_Sprint(bool isRunning)
    {
        animator.SetBool(RUN, isRunning);
    }

    private void PlayerEvents_Jump()
    {
        animator.SetTrigger(JUMP);
    }

    private void PlayerEvents_Crouch(bool isCrouching)
    {
        animator.SetBool(CROUCH, isCrouching);
    }

    public void AnimationAttackHit()
    {
        Debug.Log("Player animation event: attack hit");

        if (playerAttack != null)
        {
            playerAttack.AnimationAttackHit();
        }
    }

    public void AnimationAttackFinished()
    {
        Debug.Log("Player animation event: attack finished");

        if (playerAttack != null)
        {
            playerAttack.AnimationAttackFinished();
        }
    }
}