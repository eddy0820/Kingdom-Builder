using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;

    public Action OnWeaponSwitchedInAnimation;

    PlayerCharacterStateMachine PlayerCharacterStateMachine => PlayerController.Instance.StateMachine;

    public bool IsMovingAnimator => animator.GetBool(AnimationParameters.Moving);

    private void Awake()
    {
        PlayerCharacterStateMachine.OnGroundedMovementSprinting += () => ToggleSpint(true);
        PlayerCharacterStateMachine.OnGroundedMovementNotSprinting += () => ToggleSpint(false);

        PlayerCharacterStateMachine.OnGroundedMovementCrouching += () => ToggleCrouch(true);
        PlayerCharacterStateMachine.OnGroundedMovementNotCrouching += () => ToggleCrouch(false);

        animator.SetInteger(AnimationParameters.FromWeaponTypeSwitch, -1);
    }

    public void ToggleCrouch(bool isCrouch)
    {
        animator.SetBool(AnimationParameters.Crouch, isCrouch);
    }

    public void ToggleMoving(bool isMoving)
    {
        animator.SetBool(AnimationParameters.Moving, isMoving);
    }

    public void SetVelocityZ(float velocityZ)
    {
        animator.SetFloat(AnimationParameters.VelocityZ, velocityZ);
    }

    public void SetVelocityX(float velocityX)
    {
        animator.SetFloat(AnimationParameters.VelocityX, velocityX);
    }

    public void ToggleSpint(bool isSprint)
    {
        animator.SetBool(AnimationParameters.Sprint, isSprint);
    }

    public void SetJumpStatus(EJumpStatus jumpStatus)
    {
        animator.SetInteger(AnimationParameters.JumpNumber, (int)jumpStatus);
        animator.SetAnimatorTrigger(EAnimatorTrigger.JumpTrigger);
    }

    public void SwitchWeapon()
    {
        OnWeaponSwitchedInAnimation?.Invoke();
    }

    public void SetCurrentWeapon(EAnimatorWeaponType currentWeaponType, bool forInstantSwitch)
    {
        animator.SetInteger(AnimationParameters.CurrentWeaponType, (int)currentWeaponType);

        if(forInstantSwitch)
            animator.SetAnimatorTrigger(EAnimatorTrigger.InstantSwitchTrigger);
    }

    public void SetSheath()
    {
        animator.SetAnimatorTrigger(EAnimatorTrigger.WeaponSheathTrigger);
    }

    public void SetUnsheath()
    {
        animator.SetAnimatorTrigger(EAnimatorTrigger.WeaponUnsheathTrigger);
    }
}
