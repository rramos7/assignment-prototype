using System.Collections;
using UnityEngine;

/// <summary>
/// This base state is for shared activity and methods by every other state
/// It also takes care of the context, with the constructor referencing the
/// specific enemy controller object.
/// </summary>
public class EnemySpiderState : State
{
    public string name;
    protected EnemySpiderControllerStateMachine controller;

    protected EnemySpiderState(EnemySpiderControllerStateMachine enemySpiderControllerStateMachine)
    {
        controller = enemySpiderControllerStateMachine;
    }

    protected void UpdateFacing()
    {
        float horizontalSpeed = controller.rb.velocity.x;
        if (horizontalSpeed > 0 && !controller.isFacingRight
            || horizontalSpeed < 0 && controller.isFacingRight)
        {
            controller.FlipSprite();
        }
    }

    public override IEnumerator OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Player") == true)
        {
            other.transform.GetComponent<HealthSystem>()?.Damage(3);
            Vector2 awayDirection = other.transform.position - controller.transform.position;
            other.transform.GetComponent<PlayerController>().KickBack(awayDirection * 3f);
        }

        return base.OnCollisionEnter2D(other);
    }
}