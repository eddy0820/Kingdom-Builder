using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DecentralizedStateMachine<T> : StateMachine<T> where T : DecentralizedStateMachine<T>.DecentralizedState
{
    protected override void OnStart()
    {
        SwitchState(startingState);
    }

    public virtual void SwitchState(T nextState)
    {
        T previousState = currentState;

        currentState = nextState;
        
        previousState?.OnExitStateEvent?.Invoke(currentState);
        previousState?.OnExitState(currentState);
        currentState?.OnEnterStateEvent?.Invoke(previousState);
        currentState?.OnEnterState(previousState);
    }

    public abstract class DecentralizedState : State {}
}
