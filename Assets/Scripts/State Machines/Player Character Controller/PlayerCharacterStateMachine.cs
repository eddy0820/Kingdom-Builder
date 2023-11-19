using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerCharacterController))]
public class PlayerCharacterStateMachine : DecentralizedStateMachine<PlayerCharacterControllerState>
{
    protected override void OnAwake() {}

    protected override void OnStart() 
    {
        SwitchState(GetState(out DefaultCharacterControllerState defaultState) ? defaultState : null);
    }

    protected override void OnUpdate() {}
}
