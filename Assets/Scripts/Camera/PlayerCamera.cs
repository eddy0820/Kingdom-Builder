﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Cinemachine;
using DG.Tweening;

public class PlayerCamera : MonoBehaviour
{
    public CinemachineVirtualCamera FollowCamera;
    public CinemachineVirtualCamera LockOnCamera;

    [Header("Follow Camera Framing")]
    public Vector2 DefaultFollowPointFraming = new Vector2(0f, 0f);
    public Vector2 BuildModeFollowPointFraming = new Vector2(0f, 0f);
    [SerializeField] float flipAlignmentTweenDuration = 1f;
    [SerializeField] float buildModeCameraTweenDuration = 1f;
    public float FollowingSharpness = 10000f;
    [ReadOnly, SerializeField] Vector2 FollowPointFraming = new Vector2(0f, 0f);

    [Header("Distance")]
    public float DefaultDistance = 6f;
    public float MinDistance = 0f;
    public float MaxDistance = 10f;

    [Header("Follow Camera Distance")]
    public float DistanceMovementSpeed = 5f;
    public float DistanceMovementSharpness = 10f;

    [Header("Sensitivity")]
    [Range(0, 100)]
    [SerializeField] float sensitivityX = 8;
    public float SensitivityX => sensitivityX / 1000;
    [Range(0, 100)]
    [SerializeField] float sensitivityY = 0.075f;
    public float SensitivityY => sensitivityY / 1000;
    [SerializeField] float scrollSensitivity = 480;
    public float ScrollSensitivity => scrollSensitivity;

    [Header("Rotation")]
    public bool InvertX = false;
    public bool InvertY = false;
    [Range(-90f, 90f)]
    public float DefaultVerticalAngle = 20f;
    [Range(-90f, 90f)]
    public float MinVerticalAngle = -90f;
    [Range(-90f, 90f)]
    public float MaxVerticalAngle = 90f;
    public float RotationSpeed = 1f;
    public float RotationSharpness = 10000f;
    public bool RotateWithPhysicsMover = false;

    [Header("Obstruction")]
    public float ObstructionCheckRadius = 0.2f;
    public LayerMask ObstructionLayers = -1;
    public float ObstructionSharpness = 10000f;
    public List<Collider> IgnoredColliders = new List<Collider>();

    [Header("Camera Switch")]

    [ReadOnly] public bool inFirstPerson;
    [ReadOnly] public bool rightAligned;

    public Transform FollowCameraTransform { get; private set; }
    public Transform LockOnCameraTransform { get; private set; }
    public Transform FollowTransform { get; private set; }

    public Vector3 PlanarDirection { get; set; }
    public float TargetDistance { get; set; }

    private bool _distanceIsObstructed;
    private float _currentDistance;
    private float _targetVerticalAngle;
    private RaycastHit _obstructionHit;
    private int _obstructionCount;
    private RaycastHit[] _obstructions = new RaycastHit[MaxObstructions];
    private float _obstructionTime;
    private Vector3 _currentFollowPosition;

    CinemachineFramingTransposer lockOnFramingTransposer;

    private const int MaxObstructions = 32;

    public Action OnEnterFirstPerson;
    public Action OnExitFirstPerson;

    void OnValidate()
    {
        DefaultDistance = Mathf.Clamp(DefaultDistance, MinDistance, MaxDistance);
        DefaultVerticalAngle = Mathf.Clamp(DefaultVerticalAngle, MinVerticalAngle, MaxVerticalAngle);
    }

    void Awake()
    {
        lockOnFramingTransposer = LockOnCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        
        FollowCameraTransform = FollowCamera.transform;
        LockOnCameraTransform = LockOnCamera.transform;

        _currentDistance = DefaultDistance;
        lockOnFramingTransposer.m_CameraDistance = DefaultDistance;
        TargetDistance = _currentDistance;

        rightAligned = true;

        _targetVerticalAngle = 0f;

        PlanarDirection = Vector3.forward;

        FollowPointFraming = DefaultFollowPointFraming;

        OnEnterFirstPerson += EnterFirstPerson;
        OnExitFirstPerson += ExitFirstPerson;
    }

    // Set the transform that the camera will orbit around
    public void SetFollowTransform(Transform t)
    {
        FollowTransform = t;
        PlanarDirection = FollowTransform.forward;
        _currentFollowPosition = FollowTransform.position;
    }

    public void UpdateWithInput(float deltaTime, float zoomInput, Vector3 rotationInput)
    {
        if(FollowTransform)
        {
            if(InvertX)
            {
                rotationInput.x *= -1f;
            }
            if(InvertY)
            {
                rotationInput.y *= -1f;
            }

            // Process rotation input
            Quaternion rotationFromInput = Quaternion.Euler(FollowTransform.up * (rotationInput.x * RotationSpeed));

            if(rotationInput != Vector3.zero)
                PlanarDirection = rotationFromInput * PlanarDirection;
            
            
            PlanarDirection = Vector3.Cross(FollowTransform.up, Vector3.Cross(PlanarDirection, FollowTransform.up));
            Quaternion planarRot = Quaternion.LookRotation(PlanarDirection, FollowTransform.up);

            _targetVerticalAngle -= (rotationInput.y * RotationSpeed);
            _targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle, MinVerticalAngle, MaxVerticalAngle);
            Quaternion verticalRot = Quaternion.Euler(_targetVerticalAngle, 0, 0);
            Quaternion targetRotation = Quaternion.Slerp(FollowCameraTransform.rotation, planarRot * verticalRot, 1f - Mathf.Exp(-RotationSharpness * deltaTime));

            // Apply rotation

            FollowCameraTransform.rotation = targetRotation;

            // Process distance input
            if(_distanceIsObstructed && Mathf.Abs(zoomInput) > 0f && !PlayerController.Instance.LockedOn)
            {
                TargetDistance = _currentDistance;
            }

            TargetDistance += zoomInput * DistanceMovementSpeed;
            TargetDistance = Mathf.RoundToInt(TargetDistance);
            TargetDistance = Mathf.Clamp(TargetDistance, MinDistance, MaxDistance);

            // Find the smoothed follow position
            _currentFollowPosition = Vector3.Lerp(_currentFollowPosition, FollowTransform.position, 1f - Mathf.Exp(-FollowingSharpness * deltaTime));

            // Handle obstructions
            
            RaycastHit closestHit = new RaycastHit();
            closestHit.distance = Mathf.Infinity;
            _obstructionCount = Physics.SphereCastNonAlloc(_currentFollowPosition, ObstructionCheckRadius, -FollowCameraTransform.forward, _obstructions, TargetDistance, ObstructionLayers, QueryTriggerInteraction.Ignore);
            
            for(int i = 0; i < _obstructionCount; i++)
            {
                bool isIgnored = false;

                for(int j = 0; j < IgnoredColliders.Count; j++)
                {
                    if(IgnoredColliders[j] == _obstructions[i].collider)
                    {
                        isIgnored = true;
                        break;
                    }
                }

                for(int j = 0; j < IgnoredColliders.Count; j++)
                {
                    if(IgnoredColliders[j] == _obstructions[i].collider)
                    {
                        isIgnored = true;
                        break;
                    }
                }

                if(!isIgnored && _obstructions[i].distance < closestHit.distance && _obstructions[i].distance > 0)
                {
                    closestHit = _obstructions[i];
                }
            }

            // If obstructions detecter
            if(closestHit.distance < Mathf.Infinity)
            {
                _distanceIsObstructed = true;
                _currentDistance = Mathf.Lerp(_currentDistance, closestHit.distance, 1 - Mathf.Exp(-ObstructionSharpness * deltaTime));
            }
            // If no obstruction
            else
            {
                _distanceIsObstructed = false;
                _currentDistance = Mathf.Lerp(_currentDistance, TargetDistance, 1 - Mathf.Exp(-DistanceMovementSharpness * deltaTime));
            }
            

            // Find the smoothed camera orbit position
            Vector3 targetPosition = _currentFollowPosition - (targetRotation * Vector3.forward * _currentDistance);

            // Handle framing
            targetPosition += FollowCameraTransform.right * FollowPointFraming.x;
            targetPosition += FollowCameraTransform.up * FollowPointFraming.y;

            // Apply position

            FollowCameraTransform.position = targetPosition;

            lockOnFramingTransposer.m_CameraDistance = TargetDistance;

            if(PlayerController.Instance.LockedOn && !GetComponentInChildren<CinemachineStateDrivenCamera>().IsBlending)
            {
                FollowCameraTransform.SetPositionAndRotation(Camera.main.transform.position, Camera.main.transform.rotation);
            }
        }
    }

    public void FlipCameraPerspective()
    {
        if(TargetDistance == 0f)
        {
            TargetDistance = DefaultDistance;
        }
        else
        {
            TargetDistance = 0f;
        }
    }

    public void CheckIfFirstPerson()
    {
        PlayerCharacterController character = PlayerController.Instance.Character;

        bool lastFrameFirstPersonState = inFirstPerson;

        if(TargetDistance == 0f)
        {
            inFirstPerson = true;
            character.ChangeOrientationMethod(OrientationMethod.TowardsCamera);
            character.MeshRoot.gameObject.SetActive(false);

            if(lastFrameFirstPersonState != inFirstPerson) OnEnterFirstPerson?.Invoke();
        }
        else
        {
            inFirstPerson = false;
            character.ChangeOrientationMethod(OrientationMethod.TowardsMovement);
            character.MeshRoot.gameObject.SetActive(true);
            
            if(lastFrameFirstPersonState != inFirstPerson) OnExitFirstPerson?.Invoke();
        }
    }

    public void FlipCameraAlignment()
    {
        if(inFirstPerson) return;

        rightAligned = !rightAligned;

        if(PlayerController.Instance.BuildModeEnabled)
            TweenToBuildModeFollowPointFraming(flipAlignmentTweenDuration);
        else
            TweenToDefaultFollowPointFraming(flipAlignmentTweenDuration);

    }

    private void EnterFirstPerson()
    {
        if(PlayerController.Instance.LockedOn || lockOnFramingTransposer.m_TrackedObjectOffset.y == 1f)
            DOTween.To(() => lockOnFramingTransposer.m_TrackedObjectOffset, x => lockOnFramingTransposer.m_TrackedObjectOffset = x, new Vector3(0f, 0f, 0f), 0.5f);

        TweenToZeroFollowPointFraming(buildModeCameraTweenDuration);
    }

    private void ExitFirstPerson()
    {
        if(PlayerController.Instance.LockedOn || lockOnFramingTransposer.m_TrackedObjectOffset.y == 0f)
            DOTween.To(() => lockOnFramingTransposer.m_TrackedObjectOffset, x => lockOnFramingTransposer.m_TrackedObjectOffset = x, new Vector3(0f, 1f, 0f), 0.5f);

        if(PlayerController.Instance.BuildModeEnabled)
            TweenToBuildModeFollowPointFraming(buildModeCameraTweenDuration);
        else
            TweenToDefaultFollowPointFraming(buildModeCameraTweenDuration);

    }

    public void TweenToDefaultFollowPointFraming() => TweenToDefaultFollowPointFraming(buildModeCameraTweenDuration);
    public void TweenToBuildModeFollowPointFraming() => TweenToBuildModeFollowPointFraming(buildModeCameraTweenDuration);

    private void TweenToDefaultFollowPointFraming(float duration) => DOTween.To(() => FollowPointFraming, x => FollowPointFraming = x, FlipFollowPointFramingAlignment(DefaultFollowPointFraming), duration).SetEase(Ease.Linear);
    private void TweenToBuildModeFollowPointFraming(float duration) => DOTween.To(() => FollowPointFraming, x => FollowPointFraming = x, FlipFollowPointFramingAlignment(BuildModeFollowPointFraming), duration).SetEase(Ease.Linear);

    private void TweenToZeroFollowPointFraming(float duration) => DOTween.To(() => FollowPointFraming, x => FollowPointFraming = x, new Vector2(0f, 0f), duration).SetEase(Ease.Linear);
    private Vector2 FlipFollowPointFramingAlignment(Vector2 followPointFraming)
    {
        if(rightAligned)
            return new Vector2(Mathf.Abs(followPointFraming.x), followPointFraming.y);
        else
            return new Vector2(-followPointFraming.x, followPointFraming.y);        
    }
}
