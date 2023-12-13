using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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

    Targetable currentTargetable;
    public Targetable CurrentTargetable => currentTargetable;

    Vector3 currentLockOnPosition;
    Vector3 lookAtDirectionVector;

    protected MovementSpeedSettings LockOnWalkingSpeedSettings => PlayerCharacterController.Attributes.LockOnWalkingSpeedSettings;
    protected MovementSpeedSettings LockOnRunningSpeedSettings => PlayerCharacterController.Attributes.LockOnRunningSpeedSettings;

    DefaultCharacterControllerState defaultState;

    public Action<Targetable> OnAcquiredTarget;
    public Action<Targetable> OnLostTarget;

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

        if(currentTargetable != null) OnLostTarget?.Invoke(currentTargetable);
        currentTargetable = null;
        cameraAnimator.Play(followAnimatorState);
        lockOnReticleCanvas.gameObject.SetActive(false);

        PlayerCamera.PlanarDirection = PlayerCamera.LockOnCameraTransform.forward;
    }

    public override void OnUpdateState()
    {
        if(!TargetOnRange() || Blocked(currentTargetable.LockOnLocation.position)) 
        {
            ResetTarget();
            return;
        }
                
        if(currentTargetable == null) 
        {
            ResetTarget();
            return;
        }

        currentLockOnPosition = currentTargetable.LockOnLocation.position;
        lockOnLocator.position = currentLockOnPosition;
        lockOnReticleCanvas.transform.position = currentLockOnPosition;
        lockOnReticleCanvas.transform.localScale = (Camera.main.transform.position - currentLockOnPosition).magnitude * reticleScale * Vector3.one;
        
        Vector3 playerPosition = PlayerController.Instance.Character.Motor.Transform.position;
        lookAtDirectionVector = currentLockOnPosition - playerPosition;
        lookAtDirectionVector.y = 0;
    }

    public void DecideLockOn()
    {
        if(currentTargetable != null)
        {
            //If there is already a target, Reset.
            ResetTarget();
            return;
        }

        Targetable newTargetable;
        if((newTargetable = ScanNearBy()) != null) 
            FoundTarget(newTargetable);
        else 
            ResetTarget();
    }

    private void FoundTarget(Targetable newTargetable)
    {
        currentTargetable = newTargetable;

        stateMachine.SwitchState(this);
        OnAcquiredTarget?.Invoke(currentTargetable);
    }

    private void ResetTarget()
    {
        if(stateMachine.CurrentState != this) return;

        OnLostTarget?.Invoke(currentTargetable);
        currentTargetable = null;
        
        stateMachine.SwitchState(defaultState);
    }
        

    private Targetable ScanNearBy()
    {
        Collider[] nearbyTargets = Physics.OverlapSphere(Motor.Transform.position, noticeZone, targetLayers);
        Targetable[] nearbyTargetables = nearbyTargets.Select(x => x.GetComponent<Targetable>()).Where(x => x != null).ToArray();
        float closestAngle = maxNoticeAngle;
        Targetable closestTarget = null;

        if(nearbyTargetables.Length <= 0) return null;

        for(int i = 0; i < nearbyTargetables.Length; i++)
        {
            Vector3 dir = nearbyTargetables[i].LockOnLocation.position - Camera.main.transform.position;
            dir.y = 0;
            float _angle = Vector3.Angle(Camera.main.transform.forward, dir);
            
            if (_angle < closestAngle)
            {
                closestTarget = nearbyTargetables[i];
                closestAngle = _angle;      
            }
        }

        if(closestTarget == null) return null;

        if(Blocked(closestTarget.LockOnLocation.position)) return null;

        return closestTarget;
    }

    private bool TargetOnRange()
    {
        float dis = (Motor.Transform.position - currentLockOnPosition).magnitude;

        if(dis/2 > noticeZone) 
            return false; 
        else 
            return true;
    }

    private bool Blocked(Vector3 t)
    {
        if(Physics.Linecast(Motor.Transform.position + Vector3.up * 0.5f, t, out RaycastHit hit))
            if(hit.transform.GetComponent<Targetable>() == null) 
                return true; 
        
        return false;
    }

    public override Vector3 SetLookInputVectorFromInputs(OrientationMethod orientationMethod, Vector3 cameraPlanarDirection, Vector3 normalizedMoveInputVector)
    {
        switch(orientationMethod)
        {
            case OrientationMethod.TowardsCamera:
                return cameraPlanarDirection;
            case OrientationMethod.TowardsMovement:
                if(PlayerCharacterController.IsSprinting || PlayerCharacterController.IsCrouching)
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
        if(!Application.isPlaying) return;
        Gizmos.DrawWireSphere(Motor.Transform.position, noticeZone);   
    }
}
