using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;

    PlayerCharacterStateMachine PlayerCharacterStateMachine => PlayerController.Instance.StateMachine;

    private void Awake()
    {
        PlayerCharacterStateMachine.OnGroundedMovementSprinting += ToggleSpint;
        PlayerCharacterStateMachine.OnGroundedMovementCrouching += ToggleCrouch;
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
}
