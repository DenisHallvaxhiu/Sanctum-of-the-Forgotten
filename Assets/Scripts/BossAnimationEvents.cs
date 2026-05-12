using UnityEngine;

public class BossAnimationEvents : MonoBehaviour
{
    private BossController bossController;
    private BossAttack bossAttack;

    private void Awake()
    {
        bossController = GetComponentInParent<BossController>();
        bossAttack = GetComponentInParent<BossAttack>();
    }

    public void FinishIntro()
    {
        if (bossController != null)
            bossController.FinishIntro();
    }

    public void AnimationAttackFinished()
    {
        if (bossController != null)
            bossController.AnimationAttackFinished();
    }

    public void AnimationPunchHit()
    {
        if (bossAttack != null)
            bossAttack.AnimationPunchHit();
    }

    public void AnimationSwipeHit()
    {
        if (bossAttack != null)
            bossAttack.AnimationSwipeHit();
    }

    public void AnimationJumpAttackHit()
    {
        if (bossAttack != null)
            bossAttack.AnimationJumpAttackHit();
    }
}