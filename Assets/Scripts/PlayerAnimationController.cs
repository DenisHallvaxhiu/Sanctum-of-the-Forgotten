using UnityEngine;

public class PlayerAnimationController : MonoBehaviour {

    private PlayerEvents playerEvents;
    private Animator animator;


    //Animator Params
    const string MOVE_STRAIGHT = "isMovingStraight";
    const string RUN = "isRunning";
    const string JUMP = "isJumping";
    const string CROUCH = "isCrouching";

    private void Awake() {
        animator = GetComponent<Animator>();
        playerEvents = GetComponentInParent<PlayerEvents>();
    }

    private void OnEnable() {
        playerEvents.Move += PlayerEvents_Move;
        playerEvents.Sprint += PlayerEvents_Sprint;
        playerEvents.Jump += PlayerEvents_Jump;
        playerEvents.Crouch += PlayerEvents_Crouch;
    }

    private void PlayerEvents_Crouch(bool isCrouching) {
        animator.SetBool(CROUCH,isCrouching);
    }

    private void OnDisable() {
        playerEvents.Move -= PlayerEvents_Move;
        playerEvents.Sprint -= PlayerEvents_Sprint;
        playerEvents.Jump -= PlayerEvents_Jump;
    }
    private void PlayerEvents_Jump() {
        animator.SetTrigger(JUMP);
    }

    private void PlayerEvents_Sprint(bool isRunning) {
        animator.SetBool(RUN,isRunning);
    }

    private void PlayerEvents_Move(bool isMoving) {
        animator.SetBool(MOVE_STRAIGHT,isMoving);
    }

}
