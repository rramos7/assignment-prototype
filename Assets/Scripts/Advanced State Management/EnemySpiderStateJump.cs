using System.Collections;
using UnityEngine;

public class EnemySpiderStateJump : EnemySpiderState
{
    public EnemySpiderStateJump(EnemySpiderControllerStateMachine enemySpiderControllerStateMachine) : base(
        enemySpiderControllerStateMachine)
    {
        name = "Jump";
    }

    public override IEnumerator EndState()
    {
        return base.EndState();
    }

    public override IEnumerator EnterState()
    {
        controller.animator.Play("fighter-attack1", 0);
        return base.EnterState();
    }

    public override IEnumerator OnCollisionEnter2D(Collision2D collision)
    {
        return base.OnCollisionEnter2D(collision);
    }

    public override IEnumerator Update()
    {
        return base.Update();
    }

    /*
    public override IEnumerator OnAnimationEnd()
    {
        controller.SetState(controller.PreviousState);
        return base.OnAnimationEnd();
    }
*/
}