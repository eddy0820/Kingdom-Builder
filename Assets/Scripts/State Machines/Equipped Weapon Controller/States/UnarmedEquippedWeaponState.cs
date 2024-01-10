using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnarmedEquippedWeaponState : EquippedWeaponState
{

    public override void OnEnterState(EquippedWeaponState fromState)
    {
        if(fromState == null) return;
        
        AnimationController.SetUnsheath();

        if(AnimationController.IsMovingAnimator)
            StartCoroutine(WaitThenSwitchWeapon());
    }

    public override void OnExitState(EquippedWeaponState toState)
    {
        
    }

    public override void SwitchWeapon()
    {
        AnimationController.SetCurrentWeapon(weaponType.ToAnimatorWeaponType(), false);
    }

    IEnumerator WaitThenSwitchWeapon()
    {
        yield return new WaitForSeconds(0.1f);
        SwitchWeapon();
        yield break;
    }
}
