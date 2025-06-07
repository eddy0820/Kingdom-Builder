using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;
using EddyLib.GameSettingsSystem;

public abstract class PlayerMovementControllerConfiguration
{
    protected Vector3 Gravity => GameSettings.GetSettings<GameplaySettings>().Gravity;

    public abstract Vector3 GetMoveVector(KinematicCharacterMotor motor, MovementAttributes attributes, Quaternion cameraPlanarRotation, Vector3 clampedMoveInputVector);

    public abstract Vector3 GetLookVector(KinematicCharacterMotor motor, MovementAttributes attributes, OrientationMethod orientationMethod, Vector3 cameraPlanarDirection, Vector3 normalizedMoveInputVector);

    public abstract void DoJump(KinematicCharacterMotor motor, MovementAttributes attributes, DoJumpParams doJumpParams);

    public abstract void DoCrouchDown(KinematicCharacterMotor motor, MovementAttributes attributes, DoCrouchDownParams doCrouchDownParams);
    public abstract void DoCrouchUp(KinematicCharacterMotor motor, MovementAttributes attributes, DoCrouchUpParams doCrouchUpParams);

    public abstract void UpdateVelocity(KinematicCharacterMotor motor, MovementAttributes attributes, UpdateVelocityParams updateVelocityParams);
    public abstract void UpdateRotation(KinematicCharacterMotor motor, MovementAttributes attributes, UpdateRotationParams updateRotationParams);

    public abstract void BeforeCharacterUpdate(KinematicCharacterMotor motor, MovementAttributes attributes, float deltaTime);
    public abstract void AfterCharacterUpdate(KinematicCharacterMotor motor, MovementAttributes attributes, AfterCharacterUpdateParams afterCharacterUpdateParams);
}
