using System;
using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;

public class GroundedPlayerMovementControllerConfiguration : PlayerMovementControllerConfiguration
{
    public static Action<Vector3, float> OnSetMovementAnim;
    public static Action<bool> OnSprinting;
    public static Action OnFalling;
    public static Action OnJump;
    public static Action<bool> OnCrouching;

#region Get Vectors

    public override Vector3 GetMoveVector(KinematicCharacterMotor motor, MovementAttributes attributes, Quaternion cameraPlanarRotation, Vector3 clampedMoveInputVector)
    {
        return cameraPlanarRotation * clampedMoveInputVector;
    }

    public override Vector3 GetLookVector(KinematicCharacterMotor motor, MovementAttributes attributes, OrientationMethod orientationMethod, Vector3 cameraPlanarDirection, Vector3 normalizedMoveInputVector)
    {
        return orientationMethod switch
        {
            OrientationMethod.TowardsCamera => cameraPlanarDirection,
            OrientationMethod.TowardsMovement => normalizedMoveInputVector,
            _ => Vector3.zero,
        };
    }

#endregion

#region Do Actions

    public override void DoJump(KinematicCharacterMotor motor, MovementAttributes movementAttributes, DoJumpParams doJumpParams)
    {
        doJumpParams.timeSinceJumpRequested = 0;
        doJumpParams.jumpRequested = true;
    }

    public override void DoCrouchDown(KinematicCharacterMotor motor, MovementAttributes attributes, DoCrouchDownParams doCrouchDownParams)
    {
        doCrouchDownParams.shouldBeCrouching = true;

        if(!doCrouchDownParams.isCrouching)
        {
            doCrouchDownParams.isCrouching = true;
            doCrouchDownParams.isWalking = false;
            doCrouchDownParams.isSprinting = false;

            float crouchedCapsuleHeight = attributes.CrouchedCapsuleHeight;
            motor.SetCapsuleDimensions(0.5f, crouchedCapsuleHeight, crouchedCapsuleHeight * 0.5f);
        }
    }

    public override void DoCrouchUp(KinematicCharacterMotor motor, MovementAttributes attributes, DoCrouchUpParams doCrouchUpParams)
    {
        doCrouchUpParams.shouldBeCrouching = false;
    }

#endregion

#region Update Velocity

    public override void UpdateVelocity(KinematicCharacterMotor motor, MovementAttributes attributes, UpdateVelocityParams updateVelocityParams)
    {
        // Ground movement
        if(motor.GroundingStatus.IsStableOnGround)
        {
            float currentVelocityMagnitude = updateVelocityParams.currentVelocity.magnitude;

            Vector3 effectiveGroundNormal = motor.GroundingStatus.GroundNormal;

            // Reorient velocity on slope
            updateVelocityParams.currentVelocity = motor.GetDirectionTangentToSurface(updateVelocityParams.currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;
    
            // Calculate target velocity
            Vector3 inputRight = Vector3.Cross(updateVelocityParams.moveInputVector, motor.CharacterUp);
            Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * updateVelocityParams.moveInputVector.magnitude;
            Vector3 targetMovementVelocity = reorientedInput * CalculateTargetSpeed(
                attributes, updateVelocityParams.IsWalking, updateVelocityParams.IsSprinting, updateVelocityParams.IsCrouching, 
                updateVelocityParams.DeltaTime, updateVelocityParams.moveInputVector, ref updateVelocityParams.targetSpeed);

            // Smooth movement Velocity
            updateVelocityParams.currentVelocity = Vector3.Lerp(updateVelocityParams.currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-attributes.StableMovementSharpness * updateVelocityParams.DeltaTime));
            OnSetMovementAnim?.Invoke(updateVelocityParams.currentVelocity, attributes.RunningSpeedSettings.Speed);

            OnSprinting?.Invoke(updateVelocityParams.IsSprinting && updateVelocityParams.moveInputVector.magnitude > 0f);
        }
        // Air movement
        else
        {
            // Add move input
            if(updateVelocityParams.moveInputVector.sqrMagnitude > 0f)
            {
                Vector3 addedVelocity = attributes.AirSpeedSettings.Acceleration * updateVelocityParams.DeltaTime * updateVelocityParams.moveInputVector;

                Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(updateVelocityParams.currentVelocity, motor.CharacterUp);

                // Limit air velocity from inputs
                if(currentVelocityOnInputsPlane.magnitude < attributes.AirSpeedSettings.Speed)
                {
                    // clamp addedVel to make total vel not exceed max vel on inputs plane
                    Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, attributes.AirSpeedSettings.Speed);
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
                if(motor.GroundingStatus.FoundAnyGround)
                {
                    if(Vector3.Dot(updateVelocityParams.currentVelocity + addedVelocity, addedVelocity) > 0f)
                    {
                        Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(motor.CharacterUp, motor.GroundingStatus.GroundNormal), motor.CharacterUp).normalized;
                        addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
                    }
                }

                // Apply added velocity
                updateVelocityParams.currentVelocity += addedVelocity;
            }

            // Gravity
            updateVelocityParams.currentVelocity += Gravity * updateVelocityParams.DeltaTime;

            // Drag
            updateVelocityParams.currentVelocity *= 1f / (1f + (attributes.Drag * updateVelocityParams.DeltaTime));

            if(updateVelocityParams.currentVelocity.y <= 0f)
                OnFalling?.Invoke();
        }

        // Handle jumping
        updateVelocityParams.jumpedThisFrame = false;
        updateVelocityParams.timeSinceJumpRequested += updateVelocityParams.DeltaTime;
        if (updateVelocityParams.jumpRequested)
        {
            // See if we actually are allowed to jump
            if(!updateVelocityParams.jumpConsumed && ((attributes.AllowJumpingWhenSliding ? motor.GroundingStatus.FoundAnyGround : motor.GroundingStatus.IsStableOnGround) || updateVelocityParams.timeSinceLastAbleToJump <= attributes.JumpPostGroundingGraceTime))
            {
                // Calculate jump direction before ungrounding
                Vector3 jumpDirection = motor.CharacterUp;
                if (motor.GroundingStatus.FoundAnyGround && !motor.GroundingStatus.IsStableOnGround)
                {
                    jumpDirection = motor.GroundingStatus.GroundNormal;
                }

                // Makes the character skip ground probing/snapping on its next update. 
                // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                motor.ForceUnground();

                // Add to the return velocity and reset jump state
                updateVelocityParams.currentVelocity += (jumpDirection * attributes.JumpUpSpeed) - Vector3.Project(updateVelocityParams.currentVelocity, motor.CharacterUp);
                updateVelocityParams.currentVelocity += updateVelocityParams.moveInputVector * attributes.JumpScalableForwardSpeed;
                updateVelocityParams.jumpRequested = false;
                updateVelocityParams.jumpConsumed = true;
                updateVelocityParams.jumpedThisFrame = true;
                OnJump?.Invoke();
            }
        }

        // Take into account additive velocity
        if(updateVelocityParams.internalVelocityAdd.sqrMagnitude > 0f)
        {
            updateVelocityParams.currentVelocity += updateVelocityParams.internalVelocityAdd;
            updateVelocityParams.internalVelocityAdd = Vector3.zero;
        }
    }



    private float CalculateTargetSpeed(MovementAttributes attributes, bool isWalking, bool isSprinting, bool isCrouching, float deltaTime, Vector3 moveInputVector, ref float targetSpeed)
    {
        ChooseTargetSpeed(attributes, isWalking, isSprinting, isCrouching, moveInputVector, out float moveSpeed, out float moveAccel);

        float speed = Mathf.Lerp(targetSpeed, moveSpeed, 1f - Mathf.Exp(-moveAccel * deltaTime));

        if(speed < 0.1f)
            speed = 0;

        if(moveSpeed - speed <= 0.0001f)
            speed = moveSpeed;
        
        targetSpeed = speed;

        return speed;
    }

    protected virtual void ChooseTargetSpeed(MovementAttributes attributes, bool isWalking, bool isSprinting, bool isCrouching, Vector3 moveInputVector, out float moveSpeed, out float moveAccel)
    {
        moveSpeed = 0;
        moveAccel = attributes.RunningSpeedSettings.Acceleration;

        if(moveInputVector.magnitude > 0)
        {
            moveSpeed = attributes.RunningSpeedSettings.Speed;

            if(isWalking)
            {
                moveSpeed = attributes.WalkingSpeedSettings.Speed;
                moveAccel = attributes.WalkingSpeedSettings.Acceleration;
            }
            else if(isSprinting)
            {
                moveSpeed = attributes.SprintingSpeedSettings.Speed;
                moveAccel = attributes.SprintingSpeedSettings.Acceleration;
            }
            else if(isCrouching)
            {
                moveSpeed = attributes.CrouchingSpeedSettings.Speed;
                moveAccel = attributes.CrouchingSpeedSettings.Acceleration;
            }
        }
    }

#endregion

#region Update Rotation

    public override void UpdateRotation(KinematicCharacterMotor motor, MovementAttributes attributes, UpdateRotationParams updateRotationParams)
    {
        if(updateRotationParams.LookInputVector.sqrMagnitude > 0f && attributes.OrientationSharpness > 0f)
        {
            // Smoothly interpolate from current to target look direction
            Vector3 smoothedLookInputDirection = Vector3.Slerp(motor.CharacterForward, updateRotationParams.LookInputVector, 1 - Mathf.Exp(-attributes.OrientationSharpness * updateRotationParams.DeltaTime)).normalized;

            // Set the current rotation (which will be used by the KinematicCharacterMotor)
            updateRotationParams.currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, motor.CharacterUp);
        }

        Vector3 currentUp = updateRotationParams.currentRotation * Vector3.up;

        if(updateRotationParams.CurrentBonusOrientationMethod == BonusOrientationMethod.TowardsGravity)
        {
            // Rotate from current up to invert gravity
            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized, 1 - Mathf.Exp(-attributes.BonusOrientationSharpness * updateRotationParams.DeltaTime));
            updateRotationParams.currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * updateRotationParams.currentRotation;
        }
        else if(updateRotationParams.CurrentBonusOrientationMethod == BonusOrientationMethod.TowardsGroundSlopeAndGravity)
        {
            if (motor.GroundingStatus.IsStableOnGround)
            {
                Vector3 initialCharacterBottomHemiCenter = motor.TransientPosition + (currentUp * motor.Capsule.radius);

                Vector3 smoothedGroundNormal = Vector3.Slerp(motor.CharacterUp, motor.GroundingStatus.GroundNormal, 1 - Mathf.Exp(-attributes.BonusOrientationSharpness * updateRotationParams.DeltaTime));
                updateRotationParams.currentRotation = Quaternion.FromToRotation(currentUp, smoothedGroundNormal) * updateRotationParams.currentRotation;

                // Move the position to create a rotation around the bottom hemi center instead of around the pivot
                motor.SetTransientPosition(initialCharacterBottomHemiCenter + (updateRotationParams.currentRotation * Vector3.down * motor.Capsule.radius));
            }
            else
            {
                Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized, 1 - Mathf.Exp(-attributes.BonusOrientationSharpness * updateRotationParams.DeltaTime));
                updateRotationParams.currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * updateRotationParams.currentRotation;
            }
        }
        else
        {
            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-attributes.BonusOrientationSharpness *  updateRotationParams.DeltaTime));
            updateRotationParams.currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * updateRotationParams.currentRotation;
        }
    }

#endregion

#region Character Updates

    public override void BeforeCharacterUpdate(KinematicCharacterMotor motor, MovementAttributes attributes, float deltaTime)
    {
        
    }

    public override void AfterCharacterUpdate(KinematicCharacterMotor motor, MovementAttributes attributes, AfterCharacterUpdateParams afterCharacterUpdateParams)
    {
        // Handle jump-related values

        // Handle jumping pre-ground grace period
        if (afterCharacterUpdateParams.jumpRequested && afterCharacterUpdateParams.TimeSinceJumpRequested > attributes.JumpPreGroundingGraceTime)
        {
            afterCharacterUpdateParams.jumpRequested = false;
        }

        if(attributes.AllowJumpingWhenSliding ? motor.GroundingStatus.FoundAnyGround : motor.GroundingStatus.IsStableOnGround)
        {
            // If we're on a ground surface, reset jumping values
            if(!afterCharacterUpdateParams.JumpedThisFrame)
            {
                afterCharacterUpdateParams.jumpConsumed = false;
            }

            afterCharacterUpdateParams.timeSicneLastAbleToJump = 0f;
        }
        else
        {
            // Keep track of time since we were last able to jump (for grace period)
            afterCharacterUpdateParams.timeSicneLastAbleToJump += afterCharacterUpdateParams.DeltaTime;
        }


        // Handle uncrouching
        if (afterCharacterUpdateParams.isCrouching && !afterCharacterUpdateParams.ShouldBeCrouching)
        {
            // Do an overlap test with the character's standing height to see if there are any obstructions
            motor.SetCapsuleDimensions(0.5f, 2f, 1f);
            if(motor.CharacterOverlap
                (
                    motor.TransientPosition,
                    motor.TransientRotation,
                    afterCharacterUpdateParams.ProbedColliders,
                    motor.CollidableLayers,
                    QueryTriggerInteraction.Ignore
                ) > 0) 
                // If obstructions, just stick to crouching dimensions
                motor.SetCapsuleDimensions(0.5f, attributes.CrouchedCapsuleHeight, attributes.CrouchedCapsuleHeight * 0.5f);
        }
        else
        {
            // If no obstructions, uncrouch
            afterCharacterUpdateParams.isCrouching = false;
        }

        OnCrouching?.Invoke(afterCharacterUpdateParams.isCrouching);
    }
}

#endregion

