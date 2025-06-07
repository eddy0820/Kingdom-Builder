using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquippedWeaponState : DecentralizedStateMachine<EquippedWeaponState>.DecentralizedState
{
    [SerializeField] protected EWeaponTypes weaponType;
    public EWeaponTypes WeaponType => weaponType;

    protected PlayerAnimationController AnimationController => PlayerController.Instance.AnimationController;

    public abstract void SwitchWeapon();
}
