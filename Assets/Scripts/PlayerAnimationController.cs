using UnityEngine;

public class PlayerAnimationController : MonoBehaviour {

    private PlayerEvents playerEvents;
    private Animator animator;


    //Animator Params
    const string MOVE = "isMoving";
    const string RUN = "isRunning";

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
        throw new System.NotImplementedException();
    }

    private void PlayerEvents_Sprint(bool isRunning) {
        animator.SetBool(RUN,isRunning);
    }

    private void PlayerEvents_Move(bool isMoving) {
        animator.SetBool(MOVE,isMoving);
    }
}
