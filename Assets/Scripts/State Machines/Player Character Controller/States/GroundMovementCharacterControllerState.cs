using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public abstract class GroundMovementCharacterControllerState : PlayerCharacterControllerState
{   
    protected MovementSpeedSettings WalkingSpeedSettings => playerCharacterController.Attributes.WalkingSpeedSettings;
    protected MovementSpeedSettings RunningSpeedSettings => playerCharacterController.Attributes.RunningSpeedSettings;
    protected MovementSpeedSettings SprintingSpeedSettings => playerCharacterController.Attributes.SprintingSpeedSettings;
    protected MovementSpeedSettings CrouchingSpeedSettings => playerCharacterController.Attributes.CrouchingSpeedSettings;

    protected float StableMovementSharpness => playerCharacterController.Attributes.StableMovementSharpness;
    protected float OrientationSharpness => playerCharacterController.Attributes.OrientationSharpness;
    protected float BonusOrientationSharpness => playerCharacterController.Attributes.BonusOrientationSharpness;
    
    protected MovementSpeedSettings AirSpeedSettings => playerCharacterController.Attributes.AirSpeedSettings;
    protected float Drag => playerCharacterController.Attributes.Drag;
    
    protected bool AllowJumpingWhenSliding => playerCharacterController.Attributes.AllowJumpingWhenSliding;
    protected float JumpUpSpeed => playerCharacterController.Attributes.JumpUpSpeed;
    protected float JumpScalableForwardSpeed => playerCharacterController.Attributes.JumpScalableForwardSpeed;
    protected float JumpPreGroundingGraceTime => playerCharacterController.Attributes.JumpPreGroundingGraceTime;
    protected float JumpPostGroundingGraceTime => playerCharacterController.Attributes.JumpPostGroundingGraceTime;

    protected float CrouchedCapsuleHeight => playerCharacterController.Attributes.CrouchedCapsuleHeight;

    protected BonusOrientationMethod CurrentBonusOrientationMethod => playerCharacterController.CurrentBonusOrientationMethod;
    protected bool IsWalking => playerCharacterController.IsWalking;
    protected bool IsSprinting => playerCharacterController.IsSprinting;
    protected bool IsCrouching => playerCharacterController.IsCrouching;

    protected PlayerAnimationController AnimationController => PlayerController.Instance.AnimationController;
    protected PlayerCamera Camera => PlayerController.Instance.CharacterCamera;
    protected KinematicCharacterMotor Motor => playerCharacterController.Motor;
    protected Transform MeshRoot => playerCharacterController.MeshRoot;
    protected Vector3 Gravity => GameSettings.Instance.Gravity;  

    public override Vector3 SetMoveInputVectorFromInputs(Quaternion cameraPlanarRotation, Vector3 clampedMoveInputVector)
    {
        return cameraPlanarRotation * clampedMoveInputVector;
    }

    public override Vector3 SetLookInputVectorFromInputs(OrientationMethod orientationMethod, Vector3 cameraPlanarDirection, Vector3 normalizedMoveInputVector)
    {
        switch(orientationMethod)
        {
            case OrientationMethod.TowardsCamera:
                return cameraPlanarDirection;
            case OrientationMethod.TowardsMovement:
                return normalizedMoveInputVector;
            default:
                return Vector3.zero;
        }
    }

    public override void DoJump(ref float timeSinceJumpRequested, ref bool jumpRequested)
    {
        timeSinceJumpRequested = 0f;
        jumpRequested = true;
    }

    public override void DoCrouchDown(ref bool shouldBeCrouching, ref bool isCrouching, ref bool isWalking, ref bool isSprinting)
    {
        shouldBeCrouching = true;
        
        if(!isCrouching)
        {
            isCrouching = true;
            AnimationController.ToggleCouch(true);
            isWalking = false;
            isSprinting = false;

            float crouchedCapsuleHeight = CrouchedCapsuleHeight;
            Motor.SetCapsuleDimensions(0.5f, crouchedCapsuleHeight, crouchedCapsuleHeight * 0.5f);
        }
    }

    public override void DoCrouchUp(ref bool shouldBeCrouching)
    {
        shouldBeCrouching = false;
        AnimationController.ToggleCouch(false);
    }

    public override void UpdateVelocity(ref Vector3 currentVelocity, ref Vector3 moveInputVector, ref bool jumpedThisFrame, ref float timeSinceJumpRequested, ref bool jumpRequested, ref bool jumpConsumed, ref float timeSinceLastAbleToJump, ref Vector3 internalVelocityAdd, ref float targetSpeed, Vector3 nonRelativeMoveInputVector, float deltaTime)
    {
        // Ground movement
        if(Motor.GroundingStatus.IsStableOnGround)
        {
            float currentVelocityMagnitude = currentVelocity.magnitude;

            Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;

            // Reorient velocity on slope
            currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;
    
            // Calculate target velocity
            Vector3 inputRight = Vector3.Cross(moveInputVector, Motor.CharacterUp);
            Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * moveInputVector.magnitude;
            Vector3 targetMovementVelocity = reorientedInput * CalculateTargetSpeed(deltaTime, moveInputVector, ref targetSpeed);

            // Smooth movement Velocity
            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));
            SetMovementAnim(currentVelocity, nonRelativeMoveInputVector);

            if(IsSprinting && moveInputVector.magnitude > 0f)
            {
                AnimationController.ToggleSpint(true);
            }
            else
            {
                AnimationController.ToggleSpint(false);
            }
        }
        // Air movement
        else
        {
            // Add move input
            if(moveInputVector.sqrMagnitude > 0f)
            {
                Vector3 addedVelocity = moveInputVector * AirSpeedSettings.Acceleration * deltaTime;

                Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, Motor.CharacterUp);

                // Limit air velocity from inputs
                if(currentVelocityOnInputsPlane.magnitude < AirSpeedSettings.Speed)
                {
                    // clamp addedVel to make total vel not exceed max vel on inputs plane
                    Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, AirSpeedSettings.Speed);
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
            currentVelocity *= 1f / (1f + (Drag * deltaTime));

            if(currentVelocity.y <= 0f)
            {
                AnimationController.SetJumpStatus(EJumpStatus.Fall);
            }
        }

        // Handle jumping
        jumpedThisFrame = false;
        timeSinceJumpRequested += deltaTime;
        if (jumpRequested)
        {
            // See if we actually are allowed to jump
            if(!jumpConsumed && ((AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround) || timeSinceLastAbleToJump <= JumpPostGroundingGraceTime))
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
                currentVelocity += (moveInputVector * JumpScalableForwardSpeed);
                jumpRequested = false;
                jumpConsumed = true;
                jumpedThisFrame = true;
                AnimationController.SetJumpStatus(EJumpStatus.Jump);
            }
        }

        // Take into account additive velocity
        if (internalVelocityAdd.sqrMagnitude > 0f)
        {
            currentVelocity += internalVelocityAdd;
            internalVelocityAdd = Vector3.zero;
        }
    }

    private float CalculateTargetSpeed(float deltaTime, Vector3 moveInputVector, ref float targetSpeed)
    {
        float moveSpeed = 0;
        float moveAccel = RunningSpeedSettings.Acceleration;

        if(moveInputVector.magnitude > 0)
        {
            moveSpeed = RunningSpeedSettings.Speed;

            if(IsWalking)
            {
                moveSpeed = WalkingSpeedSettings.Speed;
                moveAccel = WalkingSpeedSettings.Acceleration;
            }
            else if(IsSprinting)
            {
                moveSpeed = SprintingSpeedSettings.Speed;
                moveAccel = SprintingSpeedSettings.Acceleration;
            }
            else if(IsCrouching)
            {
                moveSpeed = CrouchingSpeedSettings.Speed;
                moveAccel = CrouchingSpeedSettings.Acceleration;
            }
        }

        float speed = Mathf.Lerp(targetSpeed, moveSpeed, 1f - Mathf.Exp(-moveAccel * deltaTime));

        if(speed < 0.1f)
            speed = 0;

        if(moveSpeed - speed <= 0.0001f)
            speed = moveSpeed;
        
        targetSpeed = speed;

        return speed;
    }

    protected virtual void SetMovementAnim(Vector3 currentVelocity, Vector3 nonRelativeMoveInputVector)
    {
        if(currentVelocity.magnitude > 0f)
        {
            AnimationController.ToggleMoving(true);
            AnimationController.SetVelocityZ(currentVelocity.magnitude / RunningSpeedSettings.Speed);
        }
        else
        {
            AnimationController.ToggleMoving(false);
            AnimationController.SetVelocityZ(0);
        }
    }

    public override void UpdateRotation(ref Quaternion currentRotation, Vector3 _lookInputVector, float deltaTime)
    {
        if(_lookInputVector.sqrMagnitude > 0f && OrientationSharpness > 0f)
        {
            // Smoothly interpolate from current to target look direction
            Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

            // Set the current rotation (which will be used by the KinematicCharacterMotor)
            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
        }

        Vector3 currentUp = (currentRotation * Vector3.up);

        if(CurrentBonusOrientationMethod == BonusOrientationMethod.TowardsGravity)
        {
            // Rotate from current up to invert gravity
            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -GameSettings.Instance.Gravity.normalized, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
        }
        else if(CurrentBonusOrientationMethod == BonusOrientationMethod.TowardsGroundSlopeAndGravity)
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
                Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -GameSettings.Instance.Gravity.normalized, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
            }
        }
        else
        {
            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
        }
    }
    
    public override void BeforeCharacterUpdate(float deltaTime)
    {

    }
    
    public override void AfterCharacterUpdate(ref bool _jumpRequested, ref bool _jumpConsumed, ref float _timeSinceLastAbleToJump, ref bool _isCrouching, float _timeSinceJumpRequested, bool _jumpedThisFrame, bool _shouldBeCrouching, Collider[] _probedColliders, float deltaTime)
    {
        // Handle jump-related values

        // Handle jumping pre-ground grace period
        if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
        {
            _jumpRequested = false;
        }

        if(AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
        {
            // If we're on a ground surface, reset jumping values
            if(!_jumpedThisFrame)
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
            if(Motor.CharacterOverlap
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
        };
    }

    public override void AddVelocity(ref Vector3 _internalVelocityAdd, Vector3 velocity)
    {
        _internalVelocityAdd += velocity;
    }
}
