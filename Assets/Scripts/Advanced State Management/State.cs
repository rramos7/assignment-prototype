using System.Collections;
using UnityEngine;

public abstract class State
{
    /* //In most cases you will need the context of the 
       //state machine when subclassing states from this.
       //example from PlayerIdleState class:

    public class PlayerIdleState : State
    {
        private PlayerControllerStateMachine controller;

        public PlayerIdleState(EntityStateMachine playerControllerStateMachine)
        {
            controller = (PlayerControllerStateMachine)playerControllerStateMachine;
        }

        public void Update() {
            controller.transform.Translate(5f, 0f, 0f);
        //...
    */
    public virtual IEnumerator EnterState()
    {
        yield break;
    }

    public virtual IEnumerator Update()
    {
        yield break;
    }

    public virtual IEnumerator EndState()
    {
        yield break;
    }

    public virtual IEnumerator OnCollisionEnter2D(Collision2D collision)
    {
        yield break;
    }

    public virtual IEnumerator OnTriggerEnter2D(Collider2D otherCollider)
    {
        yield break;
    }

    //When the length of an animation is used to control state changes
    //the animation needs to have an event added at the end
    //that eventually triggers this OnAnimationEnd() in the state
    public virtual IEnumerator OnAnimationEnd()
    {
        yield break;
    }

    /*
    //Could start this coroutine to begin and play an animation to the end
    //this was inconsistent because using normalizedTime can be unreliable
    //also, interruptions would be a bit unwieldy
    public IEnumerator PlayAndWaitForAnim(Animator targetAnimator, string stateName)
    {
        targetAnimator.Play(stateName);
        int failsafeCount = 0;
        //Wait until we enter the current state
        while (!targetAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            failsafeCount++; if (failsafeCount > 10) yield break; //?? good idea ??
            yield return null;
        }

        //Now, Wait until the current state is done playing (the 0.9f is problematic depending on number of frames...)
        while ((targetAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime) % 1 < 0.9f)
        {
            yield return null;
        }

        yield break;
    }
    */
}