using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class EdgeObjectColliderVisual : MonoBehaviour
{
    [SerializeField, ReadOnly] bool isCollidingWithEdgeObjectVisual;
    public bool IsCollidingWithEdgeObjectVisual => isCollidingWithEdgeObjectVisual;

    Collider currentOtherCollider;

    private void Update()
    {
        // For when the currentOtherCollider GameObject gets destroyed
        if(IsThisABuildingGhost() && isCollidingWithEdgeObjectVisual && currentOtherCollider == null)
        {
            isCollidingWithEdgeObjectVisual = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(IsThisABuildingGhost() && OtherIsEdgeObject(other))
        {
            isCollidingWithEdgeObjectVisual = true;
            currentOtherCollider = other;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(IsThisABuildingGhost() && OtherIsEdgeObject(other))
        {
            isCollidingWithEdgeObjectVisual = true;
            currentOtherCollider = other;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(IsThisABuildingGhost() && OtherIsEdgeObject(other))
        {
            isCollidingWithEdgeObjectVisual = false;
        }
    }

    private bool IsThisABuildingGhost()
    {
        return gameObject.layer == LayerMask.NameToLayer("Building Ghost");
    }

    private bool OtherIsEdgeObject(Collider other)
    {
        return other.GetComponentInParent<EdgeObjectColliderVisual>();
    }
}