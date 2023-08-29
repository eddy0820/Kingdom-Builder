using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimatorExtensions
{
    public static void SetAnimatorTrigger(this Animator animator, EAnimatorTrigger trigger)
    {
        animator.SetInteger(AnimationParameters.TriggerNumber, (int)trigger);
        animator.SetTrigger(AnimationParameters.Trigger);
    }
}
