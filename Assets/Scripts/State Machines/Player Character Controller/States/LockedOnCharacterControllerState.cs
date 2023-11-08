using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedOnCharacterControllerState : GroundMovementCharacterControllerState
{
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
            AnimationController.SetVelocityZ(relativeVelocity.z / RunningSpeedSettings.Speed);
            AnimationController.SetVelocityX(relativeVelocity.x / RunningSpeedSettings.Speed);
        }
        else
        {
            AnimationController.ToggleMoving(false);
            AnimationController.SetVelocityZ(0);
            AnimationController.SetVelocityX(0);
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
