using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;

    public void ToggleCouch(bool isCouch)
    {
        animator.SetBool(AnimationParameters.Crouch, isCouch);
    }

    public void ToggleMoving(bool isMoving)
    {
        animator.SetBool(AnimationParameters.Moving, isMoving);
    }

    public void SetVelocity(float velocityZ)
    {
        animator.SetFloat(AnimationParameters.VelocityZ, velocityZ);
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
