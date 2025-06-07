using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterCharacterUpdateParams
{
    public bool jumpRequested;
    public bool jumpConsumed;
    public float timeSicneLastAbleToJump;
    public bool isCrouching;

    readonly float timeSinceJumpRequested;
    public float TimeSinceJumpRequested => timeSinceJumpRequested;

    readonly bool jumpedThisFrame;
    public bool JumpedThisFrame => jumpedThisFrame;

    readonly bool shouldBeCrouching;
    public bool ShouldBeCrouching => shouldBeCrouching;

    readonly Collider[] probedColliders;
    public Collider[] ProbedColliders => probedColliders;

    readonly float deltaTime;
    public float DeltaTime => deltaTime;

    public AfterCharacterUpdateParams(float _timeSinceJumpRequested, bool _jumpedThisFrame, bool _shouldBeCrouching, Collider[] _probedColliders, float _deltaTime)
    {
        timeSinceJumpRequested = _timeSinceJumpRequested;
        jumpedThisFrame = _jumpedThisFrame;
        shouldBeCrouching = _shouldBeCrouching;
        probedColliders = _probedColliders;
        deltaTime = _deltaTime;
    }

}
