using UnityEngine;

[System.Serializable]
public class MovementAttributes
{
    [Header("Stable Movement")]
    [SerializeField] MovementSpeedSettings walkingSpeedSettings;
    public MovementSpeedSettings WalkingSpeedSettings => walkingSpeedSettings;
    [SerializeField] MovementSpeedSettings runningSpeedSettings;
    public MovementSpeedSettings RunningSpeedSettings => runningSpeedSettings;
    [SerializeField] MovementSpeedSettings sprintingSpeedSettings;
    public MovementSpeedSettings SprintingSpeedSettings => sprintingSpeedSettings;
    [SerializeField] MovementSpeedSettings crouchingSpeedSettings;
    public MovementSpeedSettings CrouchingSpeedSettings => crouchingSpeedSettings;
    [SerializeField] MovementSpeedSettings lockOnWalkingSpeedSettings;
    public MovementSpeedSettings LockOnWalkingSpeedSettings => lockOnWalkingSpeedSettings;
    [SerializeField] MovementSpeedSettings lockOnRunningSpeedSettings;
    public MovementSpeedSettings LockOnRunningSpeedSettings => lockOnRunningSpeedSettings;
    [Space(10)]
    [SerializeField] float stableMovementSharpness;
    public float StableMovementSharpness => stableMovementSharpness;
    [SerializeField] float orientationSharpness;
    public float OrientationSharpness => orientationSharpness;
    [SerializeField] float bonusOrientationSharpness = 20f;
    public float BonusOrientationSharpness => bonusOrientationSharpness;

    [Header("Stamina Costs")]
    [SerializeField] float sprintingStaminaCostPerSecond;
    public float SprintingStaminaCostPerSecond => sprintingStaminaCostPerSecond;
    [SerializeField] float crouchingStaminaCostPerSecond;
    public float CrouchingStaminaCostPerSecond => crouchingStaminaCostPerSecond;

    [Header("Air Movement")]
    [SerializeField] MovementSpeedSettings airSpeedSettings;
    public MovementSpeedSettings AirSpeedSettings => airSpeedSettings;
    [Space(10)]
    [SerializeField] float drag;
    public float Drag => drag;

    [Header("Jumping")]
    [SerializeField] bool allowJumpingWhenSliding;
    public bool AllowJumpingWhenSliding => allowJumpingWhenSliding;
    [SerializeField] float jumpUpSpeed;
    public float JumpUpSpeed => jumpUpSpeed;
    [SerializeField] float jumpScalableForwardSpeed;
    public float JumpScalableForwardSpeed => jumpScalableForwardSpeed;
    [SerializeField] float jumpPreGroundingGraceTime;
    public float JumpPreGroundingGraceTime => jumpPreGroundingGraceTime;
    [SerializeField] float jumpPostGroundingGraceTime;
    public float JumpPostGroundingGraceTime => jumpPostGroundingGraceTime;

    [Header("Other")]
    [SerializeField] float crouchedCapsuleHeight;
    public float CrouchedCapsuleHeight => crouchedCapsuleHeight;
}
