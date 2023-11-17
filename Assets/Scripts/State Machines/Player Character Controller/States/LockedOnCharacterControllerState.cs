using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedOnCharacterControllerState : GroundMovementCharacterControllerState
{
    protected MovementSpeedSettings LockOnWalkingSpeedSettings => playerCharacterController.Attributes.LockOnWalkingSpeedSettings;
    protected MovementSpeedSettings LockOnRunningSpeedSettings => playerCharacterController.Attributes.LockOnRunningSpeedSettings;

    public override Vector3 SetLookInputVectorFromInputs(OrientationMethod orientationMethod, Vector3 cameraPlanarDirection, Vector3 normalizedMoveInputVector)
    {
        switch(orientationMethod)
        {
            case OrientationMethod.TowardsCamera:
                return cameraPlanarDirection;
            case OrientationMethod.TowardsMovement:
                if(playerCharacterController.IsSprinting)
                    return normalizedMoveInputVector;
                else
                    return Camera.LockOnController.LookAtDirectionVector.normalized;
            default:
                return Vector3.zero;
        }
    }

    protected override void SetMovementAnim(Vector3 currentVelocity, Vector3 moveInputVector)
    {
        Vector3 relativeVelocity = Motor.Transform.InverseTransformDirection(currentVelocity);

        if(currentVelocity.magnitude > 0f)
        {
            AnimationController.ToggleMoving(true);
            AnimationController.SetVelocityZ(relativeVelocity.z / LockOnRunningSpeedSettings.Speed);
            AnimationController.SetVelocityX(relativeVelocity.x / LockOnRunningSpeedSettings.Speed);
        }
        else
        {
            AnimationController.ToggleMoving(false);
            AnimationController.SetVelocityZ(0);
            AnimationController.SetVelocityX(0);
        }
    }

    protected override void ChooseTargetSpeed(Vector3 moveInputVector, out float moveSpeed, out float moveAccel)
    {
        moveSpeed = 0;
        moveAccel = LockOnRunningSpeedSettings.Acceleration;

        if(moveInputVector.magnitude > 0)
        {
            moveSpeed = LockOnRunningSpeedSettings.Speed;

            if(IsWalking)
            {
                moveSpeed = LockOnWalkingSpeedSettings.Speed;
                moveAccel = LockOnWalkingSpeedSettings.Acceleration;
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
    }

    public override void OnEnterState(PlayerCharacterControllerState fromState)
    {
        
    }

    public override void OnExitState(PlayerCharacterControllerState toState)
    {
        AnimationController.SetVelocityX(0);
    }
}
