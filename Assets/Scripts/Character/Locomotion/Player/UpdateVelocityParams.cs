using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateVelocityParams
{
    public Vector3 currentVelocity;
    public Vector3 moveInputVector;
    public bool jumpedThisFrame;
    public float timeSinceJumpRequested;
    public bool jumpRequested;
    public bool jumpConsumed;
    public float timeSinceLastAbleToJump;
    public Vector3 internalVelocityAdd;
    public float targetSpeed;

    readonly float deltaTime;
    public float DeltaTime => deltaTime;
    
    readonly bool isWalking;
    public bool IsWalking => isWalking;
    readonly bool isSprinting;
    public bool IsSprinting => isSprinting;
    readonly bool isCrouching;
    public bool IsCrouching => isCrouching;

    public UpdateVelocityParams(float _deltaTime, bool _isWalking, bool _isSprinting, bool _isCrouching)
    {
        deltaTime = _deltaTime;
        isWalking = _isWalking;
        isSprinting = _isSprinting;
        isCrouching = _isCrouching;
    }

}
