using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class LockedOnCharacterControllerState : GroundMovementCharacterControllerState
{
    [SerializeField] Transform lockOnLocator;
    [SerializeField] Canvas lockOnReticleCanvas;

    [Header("Animator")]
    [SerializeField] Animator cameraAnimator;
    [Space(10)]
    [SerializeField] string followAnimatorState;
    [SerializeField] string lockOnAnimatorState;
    [Header("Settings")]
    [SerializeField] LayerMask targetLayers;
    [SerializeField] float reticleScale = 0.1f;
    [SerializeField] bool zeroVertLook;
    [SerializeField] float noticeZone = 10f;
    [SerializeField] float maxNoticeAngle = 60;
    [SerializeField] float lookAtSmoothing = 2;
    [Space(15)]
    [ReadOnly, SerializeField] Transform currentTarget;


    float currentYOffset;
    Vector3 currentLockOnPosition;
    Vector3 lookAtDirectionVector;

    protected MovementSpeedSettings LockOnWalkingSpeedSettings => playerCharacterController.Attributes.LockOnWalkingSpeedSettings;
    protected MovementSpeedSettings LockOnRunningSpeedSettings => playerCharacterController.Attributes.LockOnRunningSpeedSettings;

    DefaultCharacterControllerState defaultState;

    public override void OnAwake()
    {
        lockOnReticleCanvas.gameObject.SetActive(false);
        stateMachine.GetState(out defaultState);
    }

    public override void OnEnterState(PlayerCharacterControllerState fromState)
    {
        cameraAnimator.Play(lockOnAnimatorState);
        lockOnReticleCanvas.gameObject.SetActive(true);
    }

    public override void OnExitState(PlayerCharacterControllerState toState)
    {
        AnimationController.SetVelocityX(0);

        currentTarget = null;
        cameraAnimator.Play(followAnimatorState);
        lockOnReticleCanvas.gameObject.SetActive(false);

        PlayerCamera.PlanarDirection = PlayerCamera.LockOnCameraTransform.forward;
    }

    public override void OnUpdateState()
    {
        if(!TargetOnRange()) 
        {
            ResetTarget();
            return;
        }
                
        if(currentTarget == null) 
        {
            ResetTarget();
            return;
        }

        currentLockOnPosition = currentTarget.position + new Vector3(0, currentYOffset, 0);
        lockOnLocator.position = currentLockOnPosition;
        lockOnReticleCanvas.transform.position = currentLockOnPosition;
        lockOnReticleCanvas.transform.localScale = (Camera.main.transform.position - currentLockOnPosition).magnitude * reticleScale * Vector3.one;
        
        Vector3 playerPosition = PlayerController.Instance.Character.Motor.Transform.position;
        lookAtDirectionVector = currentLockOnPosition - playerPosition;
        lookAtDirectionVector.y = 0;
    }

    public void DecideLockOn()
    {
        if(currentTarget)
        {
            //If there is already a target, Reset.
            ResetTarget();
            return;
        }
        
        if(currentTarget = ScanNearBy()) 
            FoundTarget();
        else 
            ResetTarget();
    }

    private void FoundTarget()
    {
        stateMachine.SwitchState(this);
    }

    private void ResetTarget()
    {
        if(stateMachine.CurrentState != this) return;
        
        stateMachine.SwitchState(defaultState);
    }

    private Transform ScanNearBy()
    {
        Collider[] nearbyTargets = Physics.OverlapSphere(transform.position, noticeZone, targetLayers);
        float closestAngle = maxNoticeAngle;
        Transform closestTarget = null;
        if (nearbyTargets.Length <= 0) return null;

        for (int i = 0; i < nearbyTargets.Length; i++)
        {
            Vector3 dir = nearbyTargets[i].transform.position - Camera.main.transform.position;
            dir.y = 0;
            float _angle = Vector3.Angle(Camera.main.transform.forward, dir);
            
            if (_angle < closestAngle)
            {
                closestTarget = nearbyTargets[i].transform;
                closestAngle = _angle;      
            }
        }

        if (!closestTarget ) return null;
        /*float h1 = closestTarget.GetComponent<CapsuleCollider>().height;
        float h2 = closestTarget.localScale.y;
        float h = h1 * h2;
        float half_h = (h / 2) / 2;
        currentYOffset = h - half_h;
        if(zeroVertLook && currentYOffset > 1.6f && currentYOffset < 1.6f * 3) currentYOffset = 1.6f;
        Vector3 tarPos = closestTarget.position + new Vector3(0, currentYOffset, 0);
        if(Blocked(tarPos)) return null;*/
        return closestTarget;
    }

    private bool TargetOnRange()
    {
        float dis = (transform.position - currentLockOnPosition).magnitude;

        if(dis/2 > noticeZone) 
            return false; 
        else 
            return true;
    }

    private bool Blocked(Vector3 t)
    {
        RaycastHit hit;
        if(Physics.Linecast(transform.position + Vector3.up * 0.5f, t, out hit)){
            if(!hit.transform.CompareTag("Enemy")) return true;
        }
        return false;
    }

    public override Vector3 SetLookInputVectorFromInputs(OrientationMethod orientationMethod, Vector3 cameraPlanarDirection, Vector3 normalizedMoveInputVector)
    {
        switch(orientationMethod)
        {
            case OrientationMethod.TowardsCamera:
                return cameraPlanarDirection;
            case OrientationMethod.TowardsMovement:
                if(playerCharacterController.IsSprinting || playerCharacterController.IsCrouching)
                    return normalizedMoveInputVector;
                else
                    return lookAtDirectionVector.normalized;
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, noticeZone);   
    }
}
