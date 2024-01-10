using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using NaughtyAttributes;

public class PlayerCharacterController : MonoBehaviour, ICharacterController
{
    [SerializeField] KinematicCharacterMotor motor;
    public KinematicCharacterMotor Motor => motor;
    [SerializeField] Transform meshRoot;
    public Transform MeshRoot => meshRoot;
    [SerializeField] Transform cameraFollowPoint;
    public Transform CameraFollowPoint => cameraFollowPoint;

    [HorizontalLine(1)]

    [ReadOnly, SerializeField] MovementAttributes attributes;
    public MovementAttributes Attributes => attributes;

    [System.Serializable]
    public class MovementAttributes
    {
        [Header("Stable Movement")]
        [ReadOnly, SerializeField] MovementSpeedSettings walkingSpeedSettings;
        public MovementSpeedSettings WalkingSpeedSettings => walkingSpeedSettings;
        [ReadOnly, SerializeField] MovementSpeedSettings runningSpeedSettings;
        public MovementSpeedSettings RunningSpeedSettings => runningSpeedSettings;
        [ReadOnly, SerializeField] MovementSpeedSettings sprintingSpeedSettings;
         public MovementSpeedSettings SprintingSpeedSettings => sprintingSpeedSettings;
        [ReadOnly, SerializeField] MovementSpeedSettings crouchingSpeedSettings;
        public MovementSpeedSettings CrouchingSpeedSettings => crouchingSpeedSettings;
        [ReadOnly, SerializeField] MovementSpeedSettings lockOnWalkingSpeedSettings;
        public MovementSpeedSettings LockOnWalkingSpeedSettings => lockOnWalkingSpeedSettings;
        [ReadOnly, SerializeField] MovementSpeedSettings lockOnRunningSpeedSettings;
        public MovementSpeedSettings LockOnRunningSpeedSettings => lockOnRunningSpeedSettings;
        [Space(10)]
        [ReadOnly, SerializeField] float stableMovementSharpness;
        public float StableMovementSharpness => stableMovementSharpness;
        [ReadOnly, SerializeField] float orientationSharpness;
        public float OrientationSharpness => orientationSharpness;
        [ReadOnly, SerializeField] float bonusOrientationSharpness;
        public float BonusOrientationSharpness => bonusOrientationSharpness;

        [Header("Stamina Costs")]
        [ReadOnly, SerializeField] float sprintingStaminaCostPerSecond;
        public float SprintingStaminaCostPerSecond => sprintingStaminaCostPerSecond;
        [ReadOnly, SerializeField] float crouchingStaminaCostPerSecond;
        public float CrouchingStaminaCostPerSecond => crouchingStaminaCostPerSecond;

        [Header("Air Movement")]
        [ReadOnly, SerializeField] MovementSpeedSettings airSpeedSettings;
        public MovementSpeedSettings AirSpeedSettings => airSpeedSettings;
        [Space(10)]
        [ReadOnly, SerializeField] float drag;
        public float Drag => drag;

        [Header("Jumping")]
        [ReadOnly, SerializeField] bool allowJumpingWhenSliding;
        public bool AllowJumpingWhenSliding => allowJumpingWhenSliding;
        [ReadOnly, SerializeField] float jumpUpSpeed;
        public float JumpUpSpeed => jumpUpSpeed;
        [ReadOnly, SerializeField] float jumpScalableForwardSpeed;
        public float JumpScalableForwardSpeed => jumpScalableForwardSpeed;
        [ReadOnly, SerializeField] float jumpPreGroundingGraceTime;
        public float JumpPreGroundingGraceTime => jumpPreGroundingGraceTime;
        [ReadOnly, SerializeField] float jumpPostGroundingGraceTime;
        public float JumpPostGroundingGraceTime => jumpPostGroundingGraceTime;

        [Header("Other")]
        [ReadOnly, SerializeField] float crouchedCapsuleHeight;
        public float CrouchedCapsuleHeight => crouchedCapsuleHeight;

        public MovementAttributes(PlayerMovementAttributesSO attributesSO)
        {
            walkingSpeedSettings = attributesSO.WalkingSpeedSettings;
            runningSpeedSettings = attributesSO.RunningSpeedSettings;
            sprintingSpeedSettings = attributesSO.SprintingSpeedSettings;
            crouchingSpeedSettings = attributesSO.CrouchingSpeedSettings;
            lockOnWalkingSpeedSettings = attributesSO.LockOnWalkingSpeedSettings;
            lockOnRunningSpeedSettings = attributesSO.LockOnRunningSpeedSettings;
            
            stableMovementSharpness = attributesSO.StableMovementSharpness;
            orientationSharpness = attributesSO.OrientationSharpness;
            bonusOrientationSharpness = attributesSO.BonusOrientationSharpness;

            sprintingStaminaCostPerSecond = attributesSO.SprintingStaminaCostPerSecond;
            crouchingStaminaCostPerSecond = attributesSO.CrouchingStaminaCostPerSecond;

            airSpeedSettings = attributesSO.AirSpeedSettings;
            drag = attributesSO.Drag;

            allowJumpingWhenSliding = attributesSO.AllowJumpingWhenSliding;
            jumpUpSpeed = attributesSO.JumpUpSpeed;
            jumpScalableForwardSpeed = attributesSO.JumpScalableForwardSpeed;
            jumpPreGroundingGraceTime = attributesSO.JumpPreGroundingGraceTime;
            jumpPostGroundingGraceTime = attributesSO.JumpPostGroundingGraceTime;

            crouchedCapsuleHeight = attributesSO.CrouchedCapsuleHeight;

        }
    }
    
    [HorizontalLine(1)]

    [Space(10)]

    [SerializeField] OrientationMethod startingOrientationMethod = OrientationMethod.TowardsCamera;
    [SerializeField] BonusOrientationMethod startingBonusOrientationMethod = BonusOrientationMethod.None;

    [ReadOnly, SerializeField] OrientationMethod currentOrientationMethod = OrientationMethod.TowardsCamera;
    public OrientationMethod CurrentOrientationMethod => currentOrientationMethod;
    [ReadOnly, SerializeField] BonusOrientationMethod currentBonusOrientationMethod = BonusOrientationMethod.None;
    public BonusOrientationMethod CurrentBonusOrientationMethod => currentBonusOrientationMethod;

    [ReadOnly, SerializeField] float targetSpeed = 0;
    [Space(10)]
    [ReadOnly, SerializeField] bool isWalking;
    public bool IsWalking => isWalking;
    [ReadOnly, SerializeField] bool isSprinting;
    public bool IsSprinting => isSprinting;
    [ReadOnly, SerializeField] bool _isCrouching;
    public bool IsCrouching => _isCrouching;

    [Space(10)]
    [SerializeField] List<Collider> ignoredColliders = new();

    Collider[] _probedColliders = new Collider[8];
    RaycastHit[] _probedHits = new RaycastHit[8];

    Vector3 _moveInputVector;
    Vector3 _nonRelativeMoveInputVector;
    Vector3 _lookInputVector;

    bool _jumpRequested = false;
    bool _jumpConsumed = false;
    bool _jumpedThisFrame = false;
    float _timeSinceJumpRequested = Mathf.Infinity;
    float _timeSinceLastAbleToJump = 0f;

    Vector3 _internalVelocityAdd = Vector3.zero;

    bool _shouldBeCrouching = false;

    Vector3 lastInnerNormal = Vector3.zero;
    Vector3 lastOuterNormal = Vector3.zero;
    
    PlayerController PlayerController => PlayerController.Instance;
    PlayerStats PlayerStats => PlayerController.PlayerStats;
    PlayerAnimationController AnimationController => PlayerController.AnimationController;
    PlayerCharacterStateMachine StateMachine => PlayerController.StateMachine;

    private void Awake()
    {
        attributes = new MovementAttributes(PlayerController.AttributesSO.MovementAttributes as PlayerMovementAttributesSO);

        currentOrientationMethod = startingOrientationMethod;
        currentBonusOrientationMethod = startingBonusOrientationMethod;

        motor.CharacterController = this;
    }

    public void SetInputs(ref PlayerCharacterInputs inputs)
    {
        // Clamp input
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);
        _nonRelativeMoveInputVector = moveInputVector;

        // Calculate camera direction and rotation on the character plane
        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, motor.CharacterUp).normalized;

        if(cameraPlanarDirection.sqrMagnitude == 0f)
        {
            cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, motor.CharacterUp).normalized;
        }

        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, motor.CharacterUp);

        _moveInputVector = StateMachine.CurrentState.SetMoveInputVectorFromInputs(cameraPlanarRotation, moveInputVector);
        _lookInputVector = StateMachine.CurrentState.SetLookInputVectorFromInputs(currentOrientationMethod, cameraPlanarDirection, _moveInputVector.normalized);
    }

#region State Transition Stuff

    public void ChangeOrientationMethod(OrientationMethod newOrientationMethod)
    {
        currentOrientationMethod = newOrientationMethod;
    }

    public void ChangeBonusOrientationMethod(BonusOrientationMethod newBonusOrientationMethod)
    {
        currentBonusOrientationMethod = newBonusOrientationMethod;
    }

#endregion    

#region Setters

    public void DoJump()
    {
        StateMachine.CurrentState.DoJump(ref _timeSinceJumpRequested, ref _jumpRequested);
    }

    public void DoCrouchDown()
    {
        if(PlayerStats is IStamina stamina)
            if(!stamina.HasEnoughStamina(attributes.CrouchingStaminaCostPerSecond * Time.deltaTime))
                return;

        StateMachine.CurrentState.DoCrouchDown(ref _shouldBeCrouching, ref _isCrouching, ref isWalking, ref isSprinting);
    }

    public void DoCrouchUp()
    {
        StateMachine.CurrentState.DoCrouchUp(ref _shouldBeCrouching);
    }

    public void SetIsWalking(bool _bool)
    {
        if(!_isCrouching)
        {
            if(_bool)
            {
                isWalking = true;
                isSprinting = false;
            }
            else
            {
                isWalking = false;
            }
        } 
    }

    public void SetIsSprinting(bool _bool)
    {
        if(PlayerStats is IStamina stamina)
        {
            if(!stamina.HasEnoughStamina(attributes.SprintingStaminaCostPerSecond * Time.deltaTime))
            {
                isSprinting = false;
                return;
            }
        }

        if(!_isCrouching)
        {
            if(_bool)
            {
                isSprinting = true;
                isWalking = false;
            }
            else
            {
                isSprinting = false;
            }
        }  
    }

#endregion

#region Character Updates

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {   
        StateMachine.CurrentState.UpdateVelocity(ref currentVelocity, ref _moveInputVector, ref _jumpedThisFrame, ref _timeSinceJumpRequested, ref _jumpRequested, ref _jumpConsumed, ref _timeSinceLastAbleToJump, ref _internalVelocityAdd, ref targetSpeed, _nonRelativeMoveInputVector, deltaTime);
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        StateMachine.CurrentState.UpdateRotation(ref currentRotation, _lookInputVector, deltaTime);
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
        StateMachine.CurrentState.BeforeCharacterUpdate(deltaTime);
    }

    public void AfterCharacterUpdate(float deltaTime)
    {
        StateMachine.CurrentState.AfterCharacterUpdate(ref _jumpRequested, ref _jumpConsumed, ref _timeSinceLastAbleToJump, ref _isCrouching, _timeSinceJumpRequested, _jumpedThisFrame, _shouldBeCrouching, _probedColliders, deltaTime);
    }

#endregion

#region Ground Stuff

    public void PostGroundingUpdate(float deltaTime) {}

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) 
    {
        if(Time.timeSinceLevelLoad > 1 && motor.GroundingStatus.IsStableOnGround && !motor.LastGroundingStatus.IsStableOnGround)
        {
            AnimationController.SetJumpStatus(EJumpStatus.Land);
        }
    }

#endregion

#region Collision Stuff

    public void AddVelocity(Vector3 velocity)
    {
        StateMachine.CurrentState.AddVelocity(ref _internalVelocityAdd, velocity);
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        if (ignoredColliders.Count == 0)
        {
            return true;
        }

        if (ignoredColliders.Contains(coll))
        {
            return false;
        }

        return true;
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }

    public void OnDiscreteCollisionDetected(Collider hitCollider) { }

#endregion
}

public enum OrientationMethod
{
    TowardsCamera,
    TowardsMovement,
}

public struct PlayerCharacterInputs
{
    public float MoveAxisForward;
    public float MoveAxisRight;
    public Quaternion CameraRotation;
}

public enum BonusOrientationMethod
{
    None,
    TowardsGravity,
    TowardsGroundSlopeAndGravity,
}