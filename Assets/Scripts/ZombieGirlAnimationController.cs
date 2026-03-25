using UnityEngine;

public class ZombieGirlAnimationController : MonoBehaviour {

    private EnemyEvents enemyEvent;
    private Animator animator;
    const string IS_MOVING = "isMoving";

    private void Awake() {
        animator = GetComponent<Animator>();
        enemyEvent = GetComponentInParent<EnemyEvents>();
    }

    private void OnEnable() {
        enemyEvent.Move += EnemyEvent_Move;
    }

    private void EnemyEvent_Move(bool isMoving) {
        animator.SetBool(IS_MOVING,isMoving);
    }

    private void OnDisable() {
        enemyEvent.Move -= EnemyEvent_Move;
    }
}
