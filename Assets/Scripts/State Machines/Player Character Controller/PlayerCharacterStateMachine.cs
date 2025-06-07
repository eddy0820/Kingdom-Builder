using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCharacterStateMachine : DecentralizedStateMachine<PlayerCharacterState>
{
    public void SetCharacterController(PlayerCharacterController playerCharacterController)
    {
        states.ForEach(x => x.SetCharacterController(playerCharacterController));
    }

    protected override void OnStart() 
    {
        base.OnStart();
    }

    protected override void OnUpdate() {}

    protected override void OnFixedUpdate() {}
}
