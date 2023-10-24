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

    public override void OnEnterState(PlayerCharacterControllerState fromState)
    {
        
    }

    public override void OnExitState(PlayerCharacterControllerState toState)
    {
        
    }
}
