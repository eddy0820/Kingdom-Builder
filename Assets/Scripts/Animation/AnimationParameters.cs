using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationParameters
{
    public static readonly int Moving = Animator.StringToHash("Moving");
    public static readonly int Crouch = Animator.StringToHash("Crouch");
    public static readonly int Sprint = Animator.StringToHash("Sprint");
    public static readonly int VelocityX = Animator.StringToHash("Velocity X");
    public static readonly int VelocityZ = Animator.StringToHash("Velocity Z");
    public static readonly int Trigger = Animator.StringToHash("Trigger");
    public static readonly int TriggerNumber = Animator.StringToHash("TriggerNumber");
    public static readonly int JumpNumber = Animator.StringToHash("JumpNumber");
    public static readonly int WeaponType = Animator.StringToHash("WeaponType");
    public static readonly int FromWeaponTypeSwitch = Animator.StringToHash("FromWeaponTypeSwitch");
}
