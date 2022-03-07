using System;
using System.Collections;
using UnityEngine;


public abstract class StateMachine : MonoBehaviour
{
    [SerializeReference] protected State currentState;
    protected State previousState;
    private State newState;
    public State CurrentState => currentState;
    public State PreviousState => previousState;

    protected virtual void Awake()
    {
        newState = null;
        InitContext();
        InitStates();
    }

    //Instantiate and initialize what would normally be done in awake        
    protected abstract void InitContext();

    //Instantiate EntityState objects for each state
    protected abstract void InitStates();

    public void SetFirstState(State state)
    {
        newState = null;
        currentState = state;
        previousState = state;
        SetState(state);
    }

    public void SetState(State state)
    {
        newState = state; //changeover will happen in LateUpdate
    }

    protected virtual void Update()
    {
        StartCoroutine(routine: currentState.Update());
    }

    private void LateUpdate()
    {
        if (newState != null)
        {
            previousState = currentState;
            currentState = newState;
            newState = null;
            StartCoroutine(routine: previousState.EndState());
            StartCoroutine(routine: currentState.EnterState());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(routine: currentState.OnCollisionEnter2D(collision));
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        StartCoroutine(currentState.OnTriggerEnter2D(otherCollider));
    }
}