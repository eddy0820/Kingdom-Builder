using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using NaughtyAttributes;


public class PlayerCharacterController : MonoBehaviour, ICharacterController, IListenToPlayerSpawn
{
    [SerializeField] KinematicCharacterMotor motor;
    public KinematicCharacterMotor Motor => motor;
    [Expandable, SerializeField] MovementAttributesSO attributesSO;
    MovementAttributes Attributes => attributesSO.Attributes;

    
    [HorizontalLine(1)]

    [Space(10)]

    [SerializeField] OrientationMethod startingOrientationMethod = OrientationMethod.TowardsCamera;
    [SerializeField] BonusOrientationMethod startingBonusOrientationMethod = BonusOrientationMethod.None;

    [ReadOnly, SerializeField] OrientationMethod currentOrientationMethod = OrientationMethod.TowardsCamera;
    [ReadOnly, SerializeField] BonusOrientationMethod currentBonusOrientationMethod = BonusOrientationMethod.None;

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

    Vector3 moveVector;
    Vector3 nonRelativeMoveVector;
    Vector3 lookVector;

    bool _jumpRequested = false;
    bool _jumpConsumed = false;
    bool _jumpedThisFrame = false;
    float _timeSinceJumpRequested = Mathf.Infinity;
    float _timeSinceLastAbleToJump = 0f;

    Vector3 _internalVelocityAdd = Vector3.zero;

    bool _shouldBeCrouching = false;

    Vector3 lastInnerNormal = Vector3.zero;
    Vector3 lastOuterNormal = Vector3.zero;

    PlayerMovementControllerConfiguration currentMovementControllerConfiguration;

    private void Awake()
    {
        currentOrientationMethod = startingOrientationMethod;
        currentBonusOrientationMethod = startingBonusOrientationMethod;

        motor.CharacterController = this;
    }

    public void OnPlayerSpawned(Transform spawnTransform)
    {
        motor.SetPositionAndRotation(spawnTransform.position, spawnTransform.rotation);
    }

    public void UpdateInputs(float moveAxisRight, float moveAxisForward, Quaternion cameraRotation)
    {
        // Clamp input
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(moveAxisRight, 0f, moveAxisForward), 1f);
        nonRelativeMoveVector = moveInputVector;

        // Calculate camera direction and rotation on the character plane
        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(cameraRotation * Vector3.forward, motor.CharacterUp).normalized;

        if(cameraPlanarDirection.sqrMagnitude == 0f)
        {
            cameraPlanarDirection = Vector3.ProjectOnPlane(cameraRotation * Vector3.up, motor.CharacterUp).normalized;
        }

        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, motor.CharacterUp);

        moveVector = currentMovementControllerConfiguration.GetMoveVector(motor, Attributes, cameraPlanarRotation, moveInputVector);
        lookVector = currentMovementControllerConfiguration.GetLookVector(motor, Attributes, currentOrientationMethod, cameraPlanarDirection, moveInputVector.normalized);
    }

    public void SetMovementControllerConfiguration(PlayerMovementControllerConfiguration newMovementControllerConfiguration)
    {
        currentMovementControllerConfiguration = newMovementControllerConfiguration;
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
        DoJumpParams doJumpParams = new()
        {
            timeSinceJumpRequested = _timeSinceJumpRequested,
            jumpRequested = _jumpRequested
        };

        currentMovementControllerConfiguration.DoJump(motor, Attributes, doJumpParams);

        _timeSinceJumpRequested = doJumpParams.timeSinceJumpRequested;
        _jumpRequested = doJumpParams.jumpRequested;
    }

    public void DoCrouchDown()
    {
        if(PlayerStats is IStamina stamina)
            if(!stamina.HasEnoughStamina(attributes.CrouchingStaminaCostPerSecond * Time.deltaTime))
                return;

        DoCrouchDownParams doCrouchDownParams = new()
        {
            shouldBeCrouching = _shouldBeCrouching,
            isCrouching = _isCrouching,
            isWalking = isWalking,
            isSprinting = isSprinting
        };

        currentMovementControllerConfiguration.DoCrouchDown(motor, Attributes, doCrouchDownParams);

        _shouldBeCrouching = doCrouchDownParams.shouldBeCrouching;
        _isCrouching = doCrouchDownParams.isCrouching;
        isWalking = doCrouchDownParams.isWalking;
        isSprinting = doCrouchDownParams.isSprinting;
    }

    public void DoCrouchUp()
    {
        DoCrouchUpParams doCrouchUpParams = new()
        {
            shouldBeCrouching = _shouldBeCrouching
        };

        currentMovementControllerConfiguration.DoCrouchUp(motor, Attributes, doCrouchUpParams);

        _shouldBeCrouching = doCrouchUpParams.shouldBeCrouching;
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
        UpdateVelocityParams updateVelocityParams = new(deltaTime, isWalking, isSprinting, _isCrouching)
        {
            currentVelocity = currentVelocity,
            moveInputVector = moveVector,
            jumpedThisFrame = _jumpedThisFrame,
            timeSinceJumpRequested = _timeSinceJumpRequested,
            jumpRequested = _jumpRequested,
            jumpConsumed = _jumpConsumed,
            timeSinceLastAbleToJump = _timeSinceLastAbleToJump,
            internalVelocityAdd = _internalVelocityAdd,
            targetSpeed = targetSpeed
        };

        currentMovementControllerConfiguration.UpdateVelocity(motor, Attributes, updateVelocityParams);

        currentVelocity = updateVelocityParams.currentVelocity;
        _jumpedThisFrame = updateVelocityParams.jumpedThisFrame;
        _timeSinceJumpRequested = updateVelocityParams.timeSinceJumpRequested;
        _jumpRequested = updateVelocityParams.jumpRequested;
        _jumpConsumed = updateVelocityParams.jumpConsumed;
        _timeSinceLastAbleToJump = updateVelocityParams.timeSinceLastAbleToJump;
        _internalVelocityAdd = updateVelocityParams.internalVelocityAdd;
        targetSpeed = updateVelocityParams.targetSpeed;
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        UpdateRotationParams updateRotationParams = new(lookVector, deltaTime, currentBonusOrientationMethod)
        {
            currentRotation = currentRotation
        };

        currentMovementControllerConfiguration.UpdateRotation(motor, Attributes, updateRotationParams);

        currentRotation = updateRotationParams.currentRotation;
    }

    public void BeforeCharacterUpdate(float deltaTime) {}

    public void AfterCharacterUpdate(float deltaTime)
    {
        AfterCharacterUpdateParams afterCharacterUpdateParams = new(_timeSinceJumpRequested, _jumpedThisFrame, _shouldBeCrouching, _probedColliders, deltaTime)
        {
            jumpRequested = _jumpRequested,
            jumpConsumed = _jumpConsumed,
            timeSicneLastAbleToJump = _timeSinceLastAbleToJump,
            isCrouching = _isCrouching
        };

        currentMovementControllerConfiguration.AfterCharacterUpdate(motor, Attributes, afterCharacterUpdateParams);

        _jumpRequested = afterCharacterUpdateParams.jumpRequested;
        _jumpConsumed = afterCharacterUpdateParams.jumpConsumed;
        _timeSinceLastAbleToJump = afterCharacterUpdateParams.timeSicneLastAbleToJump;
        _isCrouching = afterCharacterUpdateParams.isCrouching;
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
        _internalVelocityAdd += velocity;
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

public enum BonusOrientationMethod
{
    None,
    TowardsGravity,
    TowardsGroundSlopeAndGravity,
}