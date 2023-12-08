using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public abstract class MovementAttributesSO : ScriptableObject
{
    
    [Header("Stable Movement")]
    [BoxGroup(" "), SerializeField] MovementSpeedSettings walkingSpeedSettings;
    public MovementSpeedSettings WalkingSpeedSettings => walkingSpeedSettings;
    [BoxGroup(" "), SerializeField] MovementSpeedSettings runningSpeedSettings;
    public MovementSpeedSettings RunningSpeedSettings => runningSpeedSettings;
    [BoxGroup(" "), SerializeField] MovementSpeedSettings sprintingSpeedSettings;
    public MovementSpeedSettings SprintingSpeedSettings => sprintingSpeedSettings;
    [BoxGroup(" "), SerializeField] MovementSpeedSettings crouchingSpeedSettings;
    public MovementSpeedSettings CrouchingSpeedSettings => crouchingSpeedSettings;
    [BoxGroup(" "), SerializeField] MovementSpeedSettings lockOnWalkingSpeedSettings;
    public MovementSpeedSettings LockOnWalkingSpeedSettings => lockOnWalkingSpeedSettings;
    [BoxGroup(" "), SerializeField] MovementSpeedSettings lockOnRunningSpeedSettings;
    public MovementSpeedSettings LockOnRunningSpeedSettings => lockOnRunningSpeedSettings;
    [Space(10)]
    [BoxGroup(" "), SerializeField] float stableMovementSharpness;
    public float StableMovementSharpness => stableMovementSharpness;
    [BoxGroup(" "), SerializeField] float orientationSharpness;
    public float OrientationSharpness => orientationSharpness;
    [BoxGroup(" "), SerializeField] float bonusOrientationSharpness = 20f;
    public float BonusOrientationSharpness => bonusOrientationSharpness;
    [Space(10)]
    [Header("Stamina Costs")]
    [BoxGroup("     "), SerializeField] float sprintingStaminaCostPerSecond;
    public float SprintingStaminaCostPerSecond => sprintingStaminaCostPerSecond;
    [BoxGroup("     "), SerializeField] float crouchingStaminaCostPerSecond;
    public float CrouchingStaminaCostPerSecond => crouchingStaminaCostPerSecond;
    
}
