using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelaxEquippedWeaponState : EquippedWeaponState
{
    public override void OnEnterState(EquippedWeaponState fromState)
    {
        if(fromState == null) return;

        if(AnimationController.IsMovingAnimator)
            SwitchWeapon();

        AnimationController.SetSheath();
    }

    public override void OnExitState(EquippedWeaponState toState)
    {
        
    }

    public override void SwitchWeapon()
    {
        AnimationController.SetCurrentWeapon(weaponType.ToAnimatorWeaponType(), false);
    }
}
