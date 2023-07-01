using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class LooseObjectColliderVisual : AbstractColliderVisual
{
    [SerializeField, ReadOnly] bool colliding;
    public bool Colliding => colliding;

    private void OnTriggerEnter(Collider other)
    {
        if(IsThisABuildingGhost() && (OtherIsPlaceableObjectsCollider(other) || OtherIsPlaceableCollider(other))) return;

        colliding = true;   
    }

    private void OnTriggerStay(Collider other)
    {
        if(IsThisABuildingGhost() && (OtherIsPlaceableObjectsCollider(other) || OtherIsPlaceableCollider(other))) return;

        colliding = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(IsThisABuildingGhost() && (OtherIsPlaceableObjectsCollider(other) || OtherIsPlaceableCollider(other))) return;

        colliding = false;
    }
}
