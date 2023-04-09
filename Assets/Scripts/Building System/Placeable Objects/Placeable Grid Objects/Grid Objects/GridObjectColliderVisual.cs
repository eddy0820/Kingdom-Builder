using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GridObjectColliderVisual : MonoBehaviour
{
    [SerializeField, ReadOnly] bool isCollidingWithEdgeObject;
    public bool IsCollidingWithEdgeObject => isCollidingWithEdgeObject;

    Collider currentOtherCollider;

    private void Update()
    {
        // For when the currentOtherCollider GameObject gets destroyed
        if(IsThisABuildingGhost() && isCollidingWithEdgeObject && currentOtherCollider == null)
        {
            isCollidingWithEdgeObject = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(IsThisABuildingGhost() && OtherIsEdgeObject(other))
        {
            isCollidingWithEdgeObject = true;
            currentOtherCollider = other;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(IsThisABuildingGhost() && OtherIsEdgeObject(other))
        {
            isCollidingWithEdgeObject = true;
            currentOtherCollider = other;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(IsThisABuildingGhost() && OtherIsEdgeObject(other))
        {
            isCollidingWithEdgeObject = false;
        }
    }

    private bool IsThisABuildingGhost()
    {
        return gameObject.layer == LayerMask.NameToLayer("Building Ghost");
    }

    private bool OtherIsEdgeObject(Collider other)
    {
        return other.GetComponentInParent<EdgeObject>();
    }
}
