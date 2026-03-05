using UnityEngine;

public class PlayerAnimationController : MonoBehaviour {

    private PlayerEvents playerEvents;
    private Animator animator;


    //Animator Params
    const string MOVE = "isMoving";
    const string RUN = "isRunning";
    const string JUMP = "isJumping";

    private void Awake() {
        animator = GetComponent<Animator>();
        playerEvents = GetComponentInParent<PlayerEvents>();
    }

    private void OnEnable() {
        playerEvents.Move += PlayerEvents_Move;
        playerEvents.Sprint += PlayerEvents_Sprint;
        playerEvents.Jump += PlayerEvents_Jump;
    }

    private void OnDisable() {
        playerEvents.Move -= PlayerEvents_Move;
        playerEvents.Sprint -= PlayerEvents_Sprint;
        playerEvents.Jump -= PlayerEvents_Jump;
    }
    private void PlayerEvents_Jump() {
        animator.SetTrigger(JUMP);
        Debug.Log("Jump animation triggered");
    }

    private void PlayerEvents_Sprint(bool isRunning) {
        animator.SetBool(RUN,isRunning);
    }

    private void PlayerEvents_Move(bool isMoving) {
        animator.SetBool(MOVE,isMoving);
    }
}
