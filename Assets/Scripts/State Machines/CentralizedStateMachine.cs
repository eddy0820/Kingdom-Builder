using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CentralizedStateMachine<T> : StateMachine<T> where T : CentralizedStateMachine<T>.CentralizedState
{
    protected override void OnUpdate()
    {
        RunStateMachine();
    }

    protected virtual void RunStateMachine()
    {
        T nextState = currentState?.ReturnNextState();

        if(nextState != currentState && nextState != null)
            SwitchState(nextState);
    }

    protected virtual void SwitchState(T nextState)
    {
        T previousState = currentState;

        currentState = nextState;

        previousState?.OnExitStateEvent?.Invoke(currentState);
        previousState?.OnExitState(currentState);
        currentState?.OnEnterStateEvent?.Invoke(previousState);
        currentState?.OnEnterState(previousState);
    }

    public abstract class CentralizedState : State
    {
        protected new CentralizedStateMachine<T> stateMachine;

        public override void Initialize(StateMachine<T> stateMachine)
        {
            base.Initialize(stateMachine);
            this.stateMachine = stateMachine as CentralizedStateMachine<T>;
        }

        public abstract T ReturnNextState();
    }
}


