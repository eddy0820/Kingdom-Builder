using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class LooseObjectColliderVisual : MonoBehaviour
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

    private bool IsThisABuildingGhost()
    {
        return gameObject.layer == LayerMask.NameToLayer("Building Ghost");
    }

    private bool OtherIsPlaceableCollider(Collider other)
    {
        return other.gameObject.layer == LayerMask.NameToLayer("Placeable Collider");
    }

    private bool OtherIsPlaceableObjectsCollider(Collider other)
    {
        return other.gameObject.layer == LayerMask.NameToLayer("Placeable Objects Collider");
    }
}
