using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(PlayerCharacterController))]
public class PlayerCharacterStateMachine : DecentralizedStateMachine<PlayerCharacterControllerState>
{
    public Action OnGroundedMovementSprinting;
    public Action OnGroundedMovementNotSprinting;
    public Action OnGroundedMovementCrouching;
    public Action OnGroundedMovementNotCrouching;

    protected override void OnAwake() {}

    protected override void OnStart() 
    {
        SwitchState(GetState(out DefaultCharacterControllerState defaultState) ? defaultState : null);
    }

    protected override void OnUpdate() {}

    protected override void OnFixedUpdate() {}
}
