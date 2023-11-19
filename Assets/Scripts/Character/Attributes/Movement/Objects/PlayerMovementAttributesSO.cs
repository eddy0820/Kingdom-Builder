using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New Player Movement Attributes", menuName = "Attributes/Movement/PlayerMovementAttributes")]
public class PlayerMovementAttributesSO : MovementAttributesSO
{
    [Header("Air Movement")]
    [BoxGroup("  "), SerializeField] MovementSpeedSettings airSpeedSettings;
    public MovementSpeedSettings AirSpeedSettings => airSpeedSettings;
    [Space(10)]
    [BoxGroup("  "), SerializeField] float drag;
    public float Drag => drag;

    [Header("Jumping")]
    [BoxGroup("   "), SerializeField] bool allowJumpingWhenSliding;
    public bool AllowJumpingWhenSliding => allowJumpingWhenSliding;
    [BoxGroup("   "), SerializeField] float jumpUpSpeed;
    public float JumpUpSpeed => jumpUpSpeed;
    [BoxGroup("   "), SerializeField] float jumpScalableForwardSpeed;
    public float JumpScalableForwardSpeed => jumpScalableForwardSpeed;
    [BoxGroup("   "), SerializeField] float jumpPreGroundingGraceTime;
    public float JumpPreGroundingGraceTime => jumpPreGroundingGraceTime;
    [BoxGroup("   "), SerializeField] float jumpPostGroundingGraceTime;
    public float JumpPostGroundingGraceTime => jumpPostGroundingGraceTime;

    [Header("Other")]
    [BoxGroup("    "), SerializeField] float crouchedCapsuleHeight;
    public float CrouchedCapsuleHeight => crouchedCapsuleHeight;
}
