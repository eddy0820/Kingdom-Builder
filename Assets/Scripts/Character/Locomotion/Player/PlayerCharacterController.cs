﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using System;

public class PlayerCharacterController : MonoBehaviour, ICharacterController
{
    public KinematicCharacterMotor Motor;

    [Header("Stable Movement")]
    public float WalkingSpeed = 3;
    public float JoggingSpeed = 6;
    public float SprintingSpeed = 10;
    public float Acceleration = 4;
    public float Decceleration = 2;
    public float CrouchingSpeed = 3;
    public float CrouchingAcceleration = 2;
    public float CrouchingDecceleration = 2;
    
    float targetVelocity = 0;

    [Space(15)]
    public float StableMovementSharpness = 15f;
    public float OrientationSharpness = 10f;
    public OrientationMethod OrientationMethod = OrientationMethod.TowardsCamera;

    [Header("Movement Checks")]
    [ReadOnly, SerializeField] bool isWalking;
    [ReadOnly, SerializeField] bool isSprinting;
    [ReadOnly, SerializeField] bool _isCrouching;

    [Header("Air Movement")]
    public float MaxAirMoveSpeed = 15f;
    public float AirAccelerationSpeed = 15f;
    public float Drag = 0.1f;

    [Header("Jumping")]
    public bool AllowJumpingWhenSliding = false;
    public float JumpUpSpeed = 10f;
    public float JumpScalableForwardSpeed = 10f;
    public float JumpPreGroundingGraceTime = 0f;
    public float JumpPostGroundingGraceTime = 0f;

    [Header("Misc")]
    public List<Collider> IgnoredColliders = new List<Collider>();
    public BonusOrientationMethod BonusOrientationMethod = BonusOrientationMethod.None;
    public float BonusOrientationSharpness = 10f;
    public Vector3 Gravity = new Vector3(0, -30f, 0);
    public Transform MeshRoot;
    public Transform CameraFollowPoint;
    public float CrouchedCapsuleHeight = 1f;

    public CharacterState CurrentCharacterState { get; private set; }

    private Collider[] _probedColliders = new Collider[8];
    private RaycastHit[] _probedHits = new RaycastHit[8];
    private Vector3 _moveInputVector;
    private Vector3 _lookInputVector;
    private bool _jumpRequested = false;
    private bool _jumpConsumed = false;
    private bool _jumpedThisFrame = false;
    private float _timeSinceJumpRequested = Mathf.Infinity;
    private float _timeSinceLastAbleToJump = 0f;
    private Vector3 _internalVelocityAdd = Vector3.zero;
    private bool _shouldBeCrouching = false;

    private Vector3 lastInnerNormal = Vector3.zero;
    private Vector3 lastOuterNormal = Vector3.zero;

    Animator animator;
    int velocityHash;
    int crouchedHash;

    private void Awake()
    {
        TransitionToState(CharacterState.Default);

        animator = GetComponent<Animator>();
        velocityHash = Animator.StringToHash("Velocity");
        crouchedHash = Animator.StringToHash("Crouched");


        Motor.CharacterController = this;
    }

    public void TransitionToState(CharacterState newState)
    {
        CharacterState tmpInitialState = CurrentCharacterState;
        OnStateExit(tmpInitialState, newState);
        CurrentCharacterState = newState;
        OnStateEnter(newState, tmpInitialState);
    }

    public void OnStateEnter(CharacterState state, CharacterState fromState)
    {
        switch(state)
        {
            case CharacterState.Default:
            targetVelocity = 0;
                break;
        }
    }

    public void OnStateExit(CharacterState state, CharacterState toState)
    {
        switch(state)
        {
            case CharacterState.Default:
                break;
        }
    }

    public void SetInputs(ref PlayerCharacterInputs inputs)
    {
        // Clamp input
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);

        // Calculate camera direction and rotation on the character plane
        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Motor.CharacterUp).normalized;

        if(cameraPlanarDirection.sqrMagnitude == 0f)
        {
            cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, Motor.CharacterUp).normalized;
        }

        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

        switch(CurrentCharacterState)
        {
            case CharacterState.Default:
            {
                // Move and look inputs
                _moveInputVector = cameraPlanarRotation * moveInputVector;

                switch (OrientationMethod)
                {
                    case OrientationMethod.TowardsCamera:
                        _lookInputVector = cameraPlanarDirection;
                        break;
                    case OrientationMethod.TowardsMovement:
                        _lookInputVector = _moveInputVector.normalized;
                        break;
                }

                break;
            }
        }
    }

    public void DoJump()
    {
        switch(CurrentCharacterState)
        {
            case CharacterState.Default:
            {
                _timeSinceJumpRequested = 0f;
                _jumpRequested = true;
                break;
            }
        }
    }

    public void DoCrouchDown()
    {
        switch(CurrentCharacterState)
        {
            case CharacterState.Default:
            {
                _shouldBeCrouching = true;

                if(!_isCrouching)
                {
                    _isCrouching = true;
                    animator.SetBool(crouchedHash, true);
                    isWalking = false;
                    isSprinting = false;
                    Motor.SetCapsuleDimensions(0.5f, CrouchedCapsuleHeight, CrouchedCapsuleHeight * 0.5f);
                    //MeshRoot.localScale = new Vector3(1f, 0.5f, 1f);
                }

                break;
            }
        }
    }

    public void DoCrouchUp()
    {
        switch (CurrentCharacterState)
        {
            case CharacterState.Default:
            {
                _shouldBeCrouching = false;
                animator.SetBool(crouchedHash, false);
                break;
            }
        }
    }

    public void SetIsWalking(bool _bool)
    {
        if(!_isCrouching)
        {
            if(_bool && _moveInputVector != Vector3.zero)
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
        if(!_isCrouching)
        {
            if(_bool && _moveInputVector != Vector3.zero)
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

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        switch(CurrentCharacterState)
        {
            case CharacterState.Default:
            {
                // Ground movement
                if(Motor.GroundingStatus.IsStableOnGround)
                {
                    float currentVelocityMagnitude = currentVelocity.magnitude;

                    Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;

                    // Reorient velocity on slope
                    currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;
            
                    // Calculate target velocity
                    Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                    Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                    Vector3 targetMovementVelocity = reorientedInput * CalculateTargetVelocity(deltaTime);

                    // Smooth movement Velocity
                    currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));
                }
                // Air movement
                else
                {
                    // Add move input
                    if(_moveInputVector.sqrMagnitude > 0f)
                    {
                        Vector3 addedVelocity = _moveInputVector * AirAccelerationSpeed * deltaTime;

                        Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, Motor.CharacterUp);

                        // Limit air velocity from inputs
                        if(currentVelocityOnInputsPlane.magnitude < MaxAirMoveSpeed)
                        {
                            // clamp addedVel to make total vel not exceed max vel on inputs plane
                            Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, MaxAirMoveSpeed);
                            addedVelocity = newTotal - currentVelocityOnInputsPlane;
                        }
                        else
                        {
                            // Make sure added vel doesn't go in the direction of the already-exceeding velocity
                            if(Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                            {
                                addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                            }
                        }

                        // Prevent air-climbing sloped walls
                        if(Motor.GroundingStatus.FoundAnyGround)
                        {
                            if(Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
                            {
                                Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                                addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
                            }
                        }

                        // Apply added velocity
                        currentVelocity += addedVelocity;
                    }

                    // Gravity
                    currentVelocity += Gravity * deltaTime;

                    // Drag
                    currentVelocity *= (1f / (1f + (Drag * deltaTime)));
                }

                // Handle jumping
                _jumpedThisFrame = false;
                _timeSinceJumpRequested += deltaTime;
                if (_jumpRequested)
                {
                    // See if we actually are allowed to jump
                    if(!_jumpConsumed && !_isCrouching && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle->Walk->Jog->Sprint") && ((AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime))
                    {
                        // Calculate jump direction before ungrounding
                        Vector3 jumpDirection = Motor.CharacterUp;
                        if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
                        {
                            jumpDirection = Motor.GroundingStatus.GroundNormal;
                        }

                        // Makes the character skip ground probing/snapping on its next update. 
                        // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                        Motor.ForceUnground();

                        // Add to the return velocity and reset jump state
                        currentVelocity += (jumpDirection * JumpUpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                        currentVelocity += (_moveInputVector * JumpScalableForwardSpeed);
                        _jumpRequested = false;
                        _jumpConsumed = true;
                        _jumpedThisFrame = true;
                    }
                }

                // Take into account additive velocity
                if (_internalVelocityAdd.sqrMagnitude > 0f)
                {
                    currentVelocity += _internalVelocityAdd;
                    _internalVelocityAdd = Vector3.zero;
                }

                break;
            }
        }
    }

    private float CalculateTargetVelocity(float deltaTime)
    {
        if(_moveInputVector.magnitude > 0)
        {
            // Crouched
            if(_isCrouching)
            {
                if(targetVelocity >= 0 && targetVelocity < CrouchingSpeed)
                {
                    targetVelocity += deltaTime * CrouchingAcceleration;

                    if(targetVelocity > CrouchingSpeed)
                    {
                        targetVelocity = CrouchingSpeed;
                    }
                }
                else if(targetVelocity > CrouchingSpeed)
                {
                    targetVelocity = CrouchingSpeed;
                }
            }
            // Not Crouched
            else
            {
                // Between 0 and Walk Speed
                if(targetVelocity >= 0 && targetVelocity < WalkingSpeed)
                {
                    targetVelocity += deltaTime * Acceleration;

                    if(targetVelocity > WalkingSpeed)
                    {
                        targetVelocity = WalkingSpeed;
                    }
                }
                // Between Walk Speed and Jog Speed
                else if(targetVelocity >= WalkingSpeed && targetVelocity < JoggingSpeed)
                {
                    if(!isWalking)
                    {
                        targetVelocity += deltaTime * Acceleration;
                    }
                    else
                    {
                        targetVelocity -= deltaTime * Decceleration; 
                    }

                    if(targetVelocity > JoggingSpeed)
                    {
                        targetVelocity = JoggingSpeed;
                    }

                    if(targetVelocity < WalkingSpeed)
                    {
                        targetVelocity = WalkingSpeed;
                    }
                }
                // Between Jog Speed and Sprint Speed
                else if(targetVelocity >= JoggingSpeed && targetVelocity <= SprintingSpeed)
                {
                    if(isSprinting)
                    {
                        targetVelocity += deltaTime * Acceleration;
                    }
                    else
                    {
                        targetVelocity -= deltaTime * Decceleration;

                        if(!isWalking)
                        {
                            if(targetVelocity < JoggingSpeed)
                            {
                                targetVelocity = JoggingSpeed;
                            }
                        }
                    }

                    if(targetVelocity > SprintingSpeed)
                    {
                        targetVelocity = SprintingSpeed;
                    }
                }
                // Passed Sprint Speed
                else if(targetVelocity > SprintingSpeed)
                {
                    targetVelocity = SprintingSpeed;
                }
            }
        }
        else
        {
            if(_isCrouching)
            {
                if(targetVelocity > 0)
                {
                    targetVelocity -= deltaTime * CrouchingDecceleration;
                }
                else
                {
                    targetVelocity = 0;
                }
            }
            else
            {
                if(targetVelocity > 0)
                {
                    targetVelocity -= deltaTime * Decceleration;
                }
                else
                {
                    targetVelocity = 0;
                }
            }
        }

        if(_isCrouching)
        {
            animator.SetFloat(velocityHash, (targetVelocity / CrouchingSpeed));
        }
        else
        {
            animator.SetFloat(velocityHash, (targetVelocity / SprintingSpeed));
        }
        

        return targetVelocity;
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        switch (CurrentCharacterState)
        {
            case CharacterState.Default:
                {
                    if (_lookInputVector.sqrMagnitude > 0f && OrientationSharpness > 0f)
                    {
                        // Smoothly interpolate from current to target look direction
                        Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

                        // Set the current rotation (which will be used by the KinematicCharacterMotor)
                        currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
                    }

                    Vector3 currentUp = (currentRotation * Vector3.up);

                    if (BonusOrientationMethod == BonusOrientationMethod.TowardsGravity)
                    {
                        // Rotate from current up to invert gravity
                        Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                        currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                    }
                    else if (BonusOrientationMethod == BonusOrientationMethod.TowardsGroundSlopeAndGravity)
                    {
                        if (Motor.GroundingStatus.IsStableOnGround)
                        {
                            Vector3 initialCharacterBottomHemiCenter = Motor.TransientPosition + (currentUp * Motor.Capsule.radius);

                            Vector3 smoothedGroundNormal = Vector3.Slerp(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGroundNormal) * currentRotation;

                            // Move the position to create a rotation around the bottom hemi center instead of around the pivot
                            Motor.SetTransientPosition(initialCharacterBottomHemiCenter + (currentRotation * Vector3.down * Motor.Capsule.radius));
                        }
                        else
                        {
                            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                        }
                    }
                    else
                    {
                        Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                        currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                    }

                    break;
                }
        }
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
        
    }

    public void AfterCharacterUpdate(float deltaTime)
    {
        switch (CurrentCharacterState)
        {
            case CharacterState.Default:
                {
                    // Handle jump-related values

                    // Handle jumping pre-ground grace period
                    if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
                    {
                        _jumpRequested = false;
                    }

                    if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
                    {
                        // If we're on a ground surface, reset jumping values
                        if (!_jumpedThisFrame)
                        {
                            _jumpConsumed = false;
                        }
                        _timeSinceLastAbleToJump = 0f;
                    }
                    else
                    {
                        // Keep track of time since we were last able to jump (for grace period)
                        _timeSinceLastAbleToJump += deltaTime;
                    }


                    // Handle uncrouching
                    if (_isCrouching && !_shouldBeCrouching)
                    {
                        // Do an overlap test with the character's standing height to see if there are any obstructions
                        Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
                        if (Motor.CharacterOverlap
                            (
                                Motor.TransientPosition,
                                Motor.TransientRotation,
                                _probedColliders,
                                Motor.CollidableLayers,
                                QueryTriggerInteraction.Ignore
                            ) > 0)
                        {
                            // If obstructions, just stick to crouching dimensions
                            Motor.SetCapsuleDimensions(0.5f, CrouchedCapsuleHeight, CrouchedCapsuleHeight * 0.5f);
                        }
                        else
                        {
                            // If no obstructions, uncrouch
                            MeshRoot.localScale = new Vector3(1f, 1f, 1f);
                            _isCrouching = false;
                        }
                    }

                    break;
                }
        }
    }

    public void PostGroundingUpdate(float deltaTime)
    {
        // Handle landing and leaving ground
        if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
        {
            OnLanded();
        }
        else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
        {
            OnLeaveStableGround();
        }
    }

    protected void OnLanded() 
    {
        if(Time.timeSinceLevelLoad > 1)
        {
            animator.SetTrigger("Land");
        }
        
    }

    protected void OnLeaveStableGround() 
    {
        if(_jumpedThisFrame)
        {
            animator.SetTrigger("Jump");
        }
        else
        {
            animator.SetTrigger("Fall");
        }
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        if (IgnoredColliders.Count == 0)
        {
            return true;
        }

        if (IgnoredColliders.Contains(coll))
        {
            return false;
        }

        return true;
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    public void AddVelocity(Vector3 velocity)
    {
        switch (CurrentCharacterState)
        {
            case CharacterState.Default:
                {
                    _internalVelocityAdd += velocity;
                    break;
                }
        }
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }

    public void OnDiscreteCollisionDetected(Collider hitCollider) { }


}

public enum CharacterState
{
    Default,
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

public struct AICharacterInputs
{
    public Vector3 MoveVector;
    public Vector3 LookVector;
}

public enum BonusOrientationMethod
{
    None,
    TowardsGravity,
    TowardsGroundSlopeAndGravity,
}