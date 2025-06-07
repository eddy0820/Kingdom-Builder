using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippedWeaponStateMachine : DecentralizedStateMachine<EquippedWeaponState>
{
    PlayerAnimationController AnimationController => PlayerController.Instance.AnimationController;

    protected override void OnStart() 
    {
        AnimationController.OnWeaponSwitchedInAnimation += SwitchWeapon;
        AnimationController.SetCurrentWeapon(startingState.WeaponType.ToAnimatorWeaponType(), false);

        base.OnStart();
    }

    public void DecideSheath()
    {
        if(currentState is RelaxEquippedWeaponState)
            SwitchState(GetState(out UnarmedEquippedWeaponState unarmedEquippedWeaponState) ? unarmedEquippedWeaponState : null);
        else
            SwitchState(GetState(out RelaxEquippedWeaponState relaxEquippedWeaponState) ? relaxEquippedWeaponState : null);
    }

    public void SwitchWeapon()
    {
        currentState.SwitchWeapon();
    }
}
