using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;

    private void Start()
    {
        foreach(var param in animator.parameters)
        {
            Debug.Log(param.name);
        }
    }

    public void ToggleCouch(bool isCouch)
    {
        animator.SetBool(AnimationParameters.Crouch, isCouch);
    }

    public void ToggleMoving(bool isMoving, float velocityZ = 0)
    {
        animator.SetFloat(AnimationParameters.VelocityZ, velocityZ);
        animator.SetBool(AnimationParameters.Moving, isMoving);
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
