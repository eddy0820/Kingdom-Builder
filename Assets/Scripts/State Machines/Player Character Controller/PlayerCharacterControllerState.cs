using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerCharacterControllerState : DecentralizedStateMachine<PlayerCharacterControllerState>.DecentralizedState
{
    protected new PlayerCharacterStateMachine stateMachine;
    protected PlayerCharacterController PlayerCharacterController => PlayerController.Instance.Character;
    protected PlayerController PlayerController => PlayerController.Instance;

    public override void Initialize(StateMachine<PlayerCharacterControllerState> stateMachine)
    {
        base.Initialize(stateMachine);
        this.stateMachine = stateMachine as PlayerCharacterStateMachine;
    }

    public override void OnAwake()
    {

    }

    public override void OnStart()
    {
        
    }

    public override void OnUpdate()
    {
        
    }

    public override void OnUpdateState()
    {
        
    }

    public override void OnFixedUpdate()
    {
        
    }

    public override void OnFixedUpdateState()
    {
        
    }

    public abstract Vector3 SetMoveInputVectorFromInputs(Quaternion cameraPlanarRotation, Vector3 clampedMoveInputVector);
    public abstract Vector3 SetLookInputVectorFromInputs(OrientationMethod orientationMethod, Vector3 cameraPlanarDirection, Vector3 normalizedMoveInputVector);
    public abstract void DoJump(ref float timeSinceJumpRequested, ref bool jumpRequested);
    public abstract void DoCrouchDown(ref bool shouldBeCrouching, ref bool isCrouching, ref bool isWalking, ref bool isSprinting);
    public abstract void DoCrouchUp(ref bool shouldBeCrouching);
    public abstract void UpdateVelocity(ref Vector3 currentVelocity, ref Vector3 moveInputVector, ref bool jumpedThisFrame, ref float timeSinceJumpRequested, ref bool jumpRequested, ref bool jumpConsumed, ref float timeSinceLastAbleToJump, ref Vector3 internalVelocityAdd, ref float targetSpeed, Vector3 nonRelativeMoveInputVector, float deltaTime);
    public abstract void UpdateRotation(ref Quaternion currentRotation, Vector3 _lookInputVector, float deltaTime);
    public abstract void BeforeCharacterUpdate(float deltaTime);
    public abstract void AfterCharacterUpdate(ref bool _jumpRequested, ref bool _jumpConsumed, ref float _timeSinceLastAbleToJump, ref bool _isCrouching, float _timeSinceJumpRequested, bool _jumpedThisFrame, bool _shouldBeCrouching, Collider[] _probedColliders, float deltaTime);
    public abstract void AddVelocity(ref Vector3 _internalVelocityAdd, Vector3 velocity);
}
