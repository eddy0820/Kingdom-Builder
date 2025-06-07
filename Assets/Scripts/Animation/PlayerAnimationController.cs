using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

public class PlayerAnimationController : MonoBehaviour
{
    Animator animator;

    public Action OnWeaponSwitchedInAnimation;

    public bool IsMovingAnimator => animator.GetBool(AnimationParameters.Moving);

    private void Awake()
    {
        GroundedPlayerMovementControllerConfiguration.OnSetMovementAnim += OnSetMovementAnim;
        GroundedPlayerMovementControllerConfiguration.OnSprinting += ToggleSpint;
        GroundedPlayerMovementControllerConfiguration.OnFalling += OnFalling;
        GroundedPlayerMovementControllerConfiguration.OnJump += OnJump;
        GroundedPlayerMovementControllerConfiguration.OnCrouching += ToggleCrouch;

        animator.SetInteger(AnimationParameters.FromWeaponTypeSwitch, -1);
    }

    private void OnSetMovementAnim(Vector3 velocity, float maxSpeed)
    {
        if(velocity.magnitude > 0)
        {
            ToggleMoving(true);
            SetVelocityZ(velocity.magnitude / maxSpeed);
        }
        else
        {
            ToggleMoving(false);
            SetVelocityZ(0);
        }

        /* Lock On

        if(velocity.magnitude > 0)
        {
            ToggleMoving(true);
            SetVelocityZ(velocity.z / maxSpeed);
            SetVelocityX(velocity.x / maxSpeed);
        }
        else
        {
            ToggleMoving(false);
            SetVelocityZ(0);
            SetVelocityX(0);
        }
        */
    }

    private void OnFalling() => SetJumpStatus(EJumpStatus.Fall);
    private void OnJump() => SetJumpStatus(EJumpStatus.Jump);

    private void OnDestroy()
    {
        GroundedPlayerMovementControllerConfiguration.OnSetMovementAnim -= OnSetMovementAnim;
        GroundedPlayerMovementControllerConfiguration.OnSprinting -= ToggleSpint;
        GroundedPlayerMovementControllerConfiguration.OnFalling -= OnFalling;
        GroundedPlayerMovementControllerConfiguration.OnJump -= OnJump;
        GroundedPlayerMovementControllerConfiguration.OnCrouching -= ToggleCrouch;
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
