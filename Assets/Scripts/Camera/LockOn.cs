using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class LockOn : MonoBehaviour
{
    [SerializeField] Transform lockOnLocator;
    public Transform LockOnLocator => lockOnLocator;

    [Header("Animator")]
    [SerializeField] Animator cameraAnimator;
    [Space(10)]
    [SerializeField] string followAnimatorState;
    [SerializeField] string lockOnAnimatorState;

    [Header("Settings")]
    [SerializeField] LayerMask targetLayers;
    [SerializeField] bool zeroVertLook;
    [SerializeField] float noticeZone = 10f;
    [SerializeField] float maxNoticeAngle = 60;
    [SerializeField] float lookAtSmoothing = 2;
    
    
    [Space(15)]
    [ReadOnly, SerializeField] Transform currentTarget;

    Transform cam;

    float currentYOffset;
    Vector3 pos;

    private void Awake()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
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

        if(PlayerController.Instance.LockedOn) 
        {
            if(!TargetOnRange()) 
                ResetTarget();
            LookAtTarget();
        }

    }

    private void FoundTarget()
    {
        PlayerController.Instance.OnEnterLockOn?.Invoke();
        cameraAnimator.Play(lockOnAnimatorState);
        
        PlayerController.Instance.SetLockedOn(true);
    }

    void ResetTarget()
    {
        PlayerController.Instance.OnExitLockOn?.Invoke();
        currentTarget = null;
        cameraAnimator.Play(followAnimatorState);
        
        PlayerController.Instance.SetLockedOn(false);
    }

    private void LookAtTarget()
    {
        if(currentTarget == null) 
        {
            ResetTarget();
            return;
        }

        pos = currentTarget.position + new Vector3(0, currentYOffset, 0);
        lockOnLocator.position = pos;
    }

    private Transform ScanNearBy()
    {
        Collider[] nearbyTargets = Physics.OverlapSphere(transform.position, noticeZone, targetLayers);
        float closestAngle = maxNoticeAngle;
        Transform closestTarget = null;
        if (nearbyTargets.Length <= 0) return null;

        for (int i = 0; i < nearbyTargets.Length; i++)
        {
            Vector3 dir = nearbyTargets[i].transform.position - cam.position;
            dir.y = 0;
            float _angle = Vector3.Angle(cam.forward, dir);
            
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

    bool TargetOnRange()
    {
        float dis = (transform.position - pos).magnitude;

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



    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, noticeZone);   
    }
}
