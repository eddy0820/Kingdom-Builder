using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerCharacterController))]
public class PlayerCharacterStateMachine : DecentralizedStateMachine<PlayerCharacterControllerState>
{
    PlayerCharacterController playerCharacterController;

    protected override void OnAwake()
    {
        if(!SubscribeToLockOnEvents()) return;
    }

    private bool SubscribeToLockOnEvents()
    {
        if(GetState(out LockedOnCharacterControllerState lockedOnState) && GetState(out DefaultCharacterControllerState defaultState))
        {
            PlayerController.Instance.OnEnterLockOn += () => SwitchState(lockedOnState);
            PlayerController.Instance.OnExitLockOn += () => SwitchState(defaultState);

            return true;
        }

        if(!Initialized) return false;

        if(lockedOnState == null)
        {
            Debug.LogError("PlayerCharacterStateMachine: Could not find locked on state.");
            return false;
        }
        
        Debug.LogError("PlayerCharacterStateMachine: Could not find default state.");
        return false;
    }

    protected override void OnStart() 
    {
        SwitchState(GetState(out DefaultCharacterControllerState defaultState) ? defaultState : null);
    }

    protected override void OnUpdate() {}
}
