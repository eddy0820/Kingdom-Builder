using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class EdgeObjectColliderVisual : AbstractColliderVisual
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
        if(IsThisABuildingGhost() && OtherIsEdgeObjectVisual(other))
        {
            isCollidingWithEdgeObjectVisual = true;
            currentOtherCollider = other;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(IsThisABuildingGhost() && OtherIsEdgeObjectVisual(other))
        {
            isCollidingWithEdgeObjectVisual = true;
            currentOtherCollider = other;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(IsThisABuildingGhost() && OtherIsEdgeObjectVisual(other))
        {
            isCollidingWithEdgeObjectVisual = false;
        }
    }
}
