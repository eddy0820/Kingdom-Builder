using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DecentralizedStateMachine<T> : StateMachine<T> where T : DecentralizedStateMachine<T>.DecentralizedState
{
    public virtual void SwitchState(T nextState)
    {
        T previousState = currentState;
        currentState?.OnExitState(currentState);
        currentState = nextState;
        currentState?.OnEnterState(previousState);
    }

    public abstract class DecentralizedState : State
    {
        protected new DecentralizedStateMachine<T> stateMachine;

        public override void Initialize(StateMachine<T> stateMachine)
        {
            base.Initialize(stateMachine);
            this.stateMachine = stateMachine as DecentralizedStateMachine<T>;
        }
    }
}
