using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateRotationParams
{
    public Quaternion currentRotation;

    readonly Vector3 lookInputVector;
    public Vector3 LookInputVector => lookInputVector;
    readonly float deltaTime;
    public float DeltaTime => deltaTime;
    readonly BonusOrientationMethod currentBonusOrientationMethod;
    public BonusOrientationMethod CurrentBonusOrientationMethod => currentBonusOrientationMethod;

    public UpdateRotationParams(Vector3 _lookInputVector, float _deltaTime, BonusOrientationMethod _currentBonusOrientationMethod)
    {
        lookInputVector = _lookInputVector;
        deltaTime = _deltaTime;
        currentBonusOrientationMethod = _currentBonusOrientationMethod;
    }
}
