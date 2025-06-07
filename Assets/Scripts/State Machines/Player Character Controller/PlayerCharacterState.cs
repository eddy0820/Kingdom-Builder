using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerCharacterState : DecentralizedStateMachine<PlayerCharacterState>.DecentralizedState
{
    protected PlayerCharacterController playerCharacterController;

    public void SetCharacterController(PlayerCharacterController _playerCharacterController)
    {
        playerCharacterController = _playerCharacterController;
    }
}
