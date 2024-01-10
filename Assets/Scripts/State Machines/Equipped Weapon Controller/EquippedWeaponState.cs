using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquippedWeaponState : DecentralizedStateMachine<EquippedWeaponState>.DecentralizedState
{
    [SerializeField] protected EWeaponTypes weaponType;
    public EWeaponTypes WeaponType => weaponType;

    protected new EquippedWeaponStateMachine stateMachine;
    protected PlayerAnimationController AnimationController => PlayerController.Instance.AnimationController;
    

    public override void Initialize(StateMachine<EquippedWeaponState> stateMachine)
    {
        base.Initialize(stateMachine);
        this.stateMachine = stateMachine as EquippedWeaponStateMachine;
    }

    public override void OnAwake()
    {

    }

    public override void OnStart()
    {

    }

    public override void OnUpdate()
    {

    }

    public override void OnUpdateState()
    {

    }

    public override void OnFixedUpdate()
    {

    }

    public override void OnFixedUpdateState()
    {

    }

    protected void SwitchToWeaponAnims()
    {

    }

    public abstract void SwitchWeapon();
}
