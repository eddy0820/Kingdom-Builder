using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Cinemachine;
using DG.Tweening;

//LINE 169, 241, 287, 295 LOCK ON
//LINE 279, 298, End of Class BULID MODE

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Transform cameraFollowTransform;
    Vector3 currentFollowPosition;

    [SerializeField] CinemachineVirtualCamera defaultCamera;
    public Transform DefaultCameraTransform => defaultCamera.transform;

    [Header("Framing")]
    [SerializeField] Vector2 defaultFollowPointFraming;
    [SerializeField] float flipAlignmentTweenDuration = 1f;
    [SerializeField] float switchPerspectiveTweenDuration = 1f;
    [SerializeField] float followSharpness = 10000f;
    [ReadOnly, SerializeField] Vector2 currentFollowPointFraming;

    [Header("Distance")]
    [SerializeField] float defaultCameraDistance = 6f;
    [SerializeField] float minCameraDistance = 0f;
    [SerializeField] float maxCameraDistance = 10f;
    [SerializeField] float distanceMovementSpeed = 5f;
    [SerializeField] float distanceMovementSharpness = 10f;

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
    [SerializeField] bool defaultInvertX = false;
    bool invertX;
    [SerializeField] bool defaultInvertY = false;
    bool invertY;

    [Space(5)]

    [Range(-90f, 90f)]
    [SerializeField] float defaultVerticalAngle = 20f;
    [Range(-90f, 90f)]
    [SerializeField] float minVerticalAngle = -90f;
    [Range(-90f, 90f)]
    [SerializeField] float maxVerticalAngle = 90f;
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] float rotationSharpness = 10000f;
    [SerializeField] bool rotateWithPhysicsMover = false;

    [Header("Obstruction")]
    [SerializeField] List<Transform> ignoreColliderParents;
    readonly List<Collider> ignoredColliders = new();
    [SerializeField] float obstructionCheckRadius = 0.2f;
    [SerializeField] LayerMask obstructionLayers = -1;
    [SerializeField] float obstructionSharpness = 10000f;

    public bool InFirstPerson { get; private set; }
    public bool RightAligned { get; private set; }

    Vector3 planarDirection;
    float targetDistance;
    float currentDistance;
    float targetVerticalAngle;
    bool distanceIsObstructed;
    
    private const int MAX_OBSTRUCTIONS = 32;

    public Action<bool> OnToggleFirstPerson;

    private void OnValidate()
    {
        defaultCameraDistance = Mathf.Clamp(defaultCameraDistance, minCameraDistance, maxCameraDistance);
        defaultVerticalAngle = Mathf.Clamp(defaultVerticalAngle, minVerticalAngle, maxVerticalAngle);
    }

    private void Awake()
    {
        // lockOnFramingTransposer = lockOnCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        //lockOnFramingTransposer.m_CameraDistance = defaultCameraDistance;

        currentDistance = defaultCameraDistance;
        targetDistance = currentDistance;
        currentFollowPointFraming = defaultFollowPointFraming;
        targetVerticalAngle = 0f;

        invertX = defaultInvertX;
        invertY = defaultInvertY;

        RightAligned = true;
    }

    private void Start()
    {
        planarDirection = cameraFollowTransform.forward;
        currentFollowPosition = cameraFollowTransform.position;

        ignoredColliders.Clear();
        ignoreColliderParents.ForEach(parent => ignoredColliders.AddRange(parent.GetComponentsInChildren<Collider>()));
    }

    private void Update()
    {
        CheckIfFirstPerson();
    }

    public void UpdateWithInput(Quaternion? rotationDeltaFromInterpolation, Vector3? characterUp, float deltaTime, float zoomInput, Vector3 rotationInput)
    {
        // Handle rotating the camera along with physics movers
        if(rotateWithPhysicsMover && rotationDeltaFromInterpolation != null && characterUp != null)
        {
            planarDirection = rotationDeltaFromInterpolation.Value * planarDirection;
            planarDirection = Vector3.ProjectOnPlane(planarDirection, characterUp.Value).normalized;
        }

        if(!cameraFollowTransform) return;
 
        if(invertX)
            rotationInput.x *= -1f;
    
        if(invertY)
            rotationInput.y *= -1f;

        // Process rotation input
        Quaternion rotationFromInput = Quaternion.Euler(cameraFollowTransform.up * (rotationInput.x * rotationSpeed));

        if(rotationInput != Vector3.zero)
            planarDirection = rotationFromInput * planarDirection;
        
        
        planarDirection = Vector3.Cross(cameraFollowTransform.up, Vector3.Cross(planarDirection, cameraFollowTransform.up));
        Quaternion planarRot = Quaternion.LookRotation(planarDirection, cameraFollowTransform.up);

        targetVerticalAngle -= rotationInput.y * rotationSpeed;
        targetVerticalAngle = Mathf.Clamp(targetVerticalAngle, minVerticalAngle, maxVerticalAngle);
        Quaternion verticalRot = Quaternion.Euler(targetVerticalAngle, 0, 0);
        Quaternion targetRotation = Quaternion.Slerp(DefaultCameraTransform.rotation, planarRot * verticalRot, 1f - Mathf.Exp(-rotationSharpness * deltaTime));

        // Apply rotation
        DefaultCameraTransform.rotation = targetRotation;

        // Process distance input
        if(distanceIsObstructed && Mathf.Abs(zoomInput) > 0f)// && !PlayerController.Instance.LockedOn)
        {
            targetDistance = currentDistance;
        }

        targetDistance += zoomInput * distanceMovementSpeed;
        targetDistance = Mathf.RoundToInt(targetDistance);
        targetDistance = Mathf.Clamp(targetDistance, minCameraDistance, maxCameraDistance);

        // Find the smoothed follow position
        currentFollowPosition = Vector3.Lerp(currentFollowPosition, cameraFollowTransform.position, 1f - Mathf.Exp(-followSharpness * deltaTime));

        // Handle obstructions
        RaycastHit closestHit = new() { distance = Mathf.Infinity };
        RaycastHit[] obstructions = new RaycastHit[MAX_OBSTRUCTIONS];
        int obstructionCount = Physics.SphereCastNonAlloc(currentFollowPosition, obstructionCheckRadius, -DefaultCameraTransform.forward, obstructions, targetDistance, obstructionLayers, QueryTriggerInteraction.Ignore);
        
        for(int i = 0; i < obstructionCount; i++)
        {
            bool isIgnored = false;

            for(int j = 0; j < ignoredColliders.Count; j++)
            {
                if(ignoredColliders[j] == obstructions[i].collider)
                {
                    isIgnored = true;
                    break;
                }
            }

            for(int j = 0; j < ignoredColliders.Count; j++)
            {
                if(ignoredColliders[j] == obstructions[i].collider)
                {
                    isIgnored = true;
                    break;
                }
            }

            if(!isIgnored && obstructions[i].distance < closestHit.distance && obstructions[i].distance > 0)
            {
                closestHit = obstructions[i];
            }
        }

        // If obstructions detecter
        if(closestHit.distance < Mathf.Infinity)
        {
            distanceIsObstructed = true;
            currentDistance = Mathf.Lerp(currentDistance, closestHit.distance, 1 - Mathf.Exp(-obstructionSharpness * deltaTime));
        }
        // If no obstruction
        else
        {
            distanceIsObstructed = false;
            currentDistance = Mathf.Lerp(currentDistance, targetDistance, 1 - Mathf.Exp(-distanceMovementSharpness * deltaTime));
        }
        

        // Find the smoothed camera orbit position
        Vector3 targetPosition = currentFollowPosition - (targetRotation * Vector3.forward * currentDistance);

        // Handle framing
        targetPosition += DefaultCameraTransform.right * currentFollowPointFraming.x;
        targetPosition += DefaultCameraTransform.up * currentFollowPointFraming.y;

        // Apply position
        DefaultCameraTransform.position = targetPosition;

        //lockOnFramingTransposer.m_CameraDistance = TargetDistance;

        // if(PlayerController.Instance.LockedOn && !GetComponentInChildren<CinemachineStateDrivenCamera>().IsBlending)
        // {
        //     DefaultCameraTransform.SetPositionAndRotation(Camera.main.transform.position, Camera.main.transform.rotation);
        // }
    }

    public void FlipCameraPerspective()
    {
        if(targetDistance == 0f)
            targetDistance = defaultCameraDistance;
        else
            targetDistance = 0f;
    }

    public void FlipCameraAlignment()
    {
        if(InFirstPerson) return;

        RightAligned = !RightAligned;

        // if(PlayerController.Instance.BuildModeEnabled)
        //     TweenToBuildModeFollowPointFraming(flipAlignmentTweenDuration);
        // else
        TweenToDefaultFollowPointFraming(flipAlignmentTweenDuration);
    }

    private void CheckIfFirstPerson()
    {
        bool lastFrameFirstPersonState = InFirstPerson;

        InFirstPerson = targetDistance == 0f;

        if(lastFrameFirstPersonState != InFirstPerson)
        {
            OnToggleFirstPerson?.Invoke(InFirstPerson);

            if(InFirstPerson)
                SwitchToFirstPersonCamera();
            else
                SwitchToThirdPersonCamera();
        }
    }

    private void SwitchToFirstPersonCamera()
    {
        // if(PlayerController.Instance.LockedOn || lockOnFramingTransposer.m_TrackedObjectOffset.y == 1f)
        //     DOTween.To(() => lockOnFramingTransposer.m_TrackedObjectOffset, x => lockOnFramingTransposer.m_TrackedObjectOffset = x, new Vector3(0f, 0f, 0f), 0.5f);

        TweenToZeroFollowPointFraming(switchPerspectiveTweenDuration);
    }

    private void SwitchToThirdPersonCamera()
    {
        // if(PlayerController.Instance.LockedOn || lockOnFramingTransposer.m_TrackedObjectOffset.y == 0f)
        //     DOTween.To(() => lockOnFramingTransposer.m_TrackedObjectOffset, x => lockOnFramingTransposer.m_TrackedObjectOffset = x, new Vector3(0f, 1f, 0f), 0.5f);

        // if(PlayerController.Instance.BuildModeEnabled)
        //     TweenToBuildModeFollowPointFraming(buildModeCameraTweenDuration);
        // else
        TweenToDefaultFollowPointFraming(switchPerspectiveTweenDuration);
    }

    private void TweenToZeroFollowPointFraming(float duration) => DOTween.To(() => currentFollowPointFraming, x => currentFollowPointFraming = x, new Vector2(0f, 0f), duration).SetEase(Ease.Linear);
    private void TweenToDefaultFollowPointFraming(float duration) => DOTween.To(() => currentFollowPointFraming, x => currentFollowPointFraming = x, GetFollowPointFramingAlignment(defaultFollowPointFraming), duration).SetEase(Ease.Linear);

    private Vector2 GetFollowPointFramingAlignment(Vector2 followPointFraming)
    {
        if(RightAligned)
            return new Vector2(Mathf.Abs(followPointFraming.x), followPointFraming.y);
        else
            return new Vector2(-followPointFraming.x, followPointFraming.y);        
    }

    //public void TweenToDefaultFollowPointFraming() => TweenToDefaultFollowPointFraming(buildModeCameraTweenDuration);
    //public void TweenToBuildModeFollowPointFraming() => TweenToBuildModeFollowPointFraming(buildModeCameraTweenDuration); 
    //private void TweenToBuildModeFollowPointFraming(float duration) => DOTween.To(() => FollowPointFraming, x => FollowPointFraming = x, FlipFollowPointFramingAlignment(BuildModeFollowPointFraming), duration).SetEase(Ease.Linear);
}
