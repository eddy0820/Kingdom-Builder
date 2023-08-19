using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractColliderVisual : MonoBehaviour
{
    protected bool IsThisABuildingGhost()
    {
        return GridBuildingUtil.IsThisABuildingGhost(gameObject);
    }

    protected bool OtherIsEdgeObject(Collider other)
    {
        return CheckForEdgeObjectUpRecursive(other.gameObject) || CheckForEdgeObjectDownRecursive(other.gameObject);
    }

    protected bool OtherIsEdgeObjectVisual(Collider other)
    {
        if(other.gameObject.GetComponent<EdgeObjectColliderVisual>() != null && other.gameObject.layer != LayerMask.NameToLayer("Placeable Collider"))
            return true;
        else
            return false;
    }

    private bool CheckForEdgeObjectUpRecursive(GameObject targetGameObject)
    {
        if(targetGameObject.GetComponent<EdgeObject>() != null)
        {
            return true;
        }

        if(targetGameObject.transform.parent != null)
        {
            return CheckForEdgeObjectUpRecursive(targetGameObject.transform.parent.gameObject);
        }

        return false;
    }

    private bool CheckForEdgeObjectDownRecursive(GameObject targetGameObject)
    {
        if(targetGameObject.GetComponent<EdgeObject>() != null)
        {
            return true;
        }

        foreach(Transform child in targetGameObject.transform)
        {
            if(CheckForEdgeObjectDownRecursive(child.gameObject))
            {
                return true;
            }
        }

        return false;
    }

    protected bool OtherIsPlaceableCollider(Collider other)
    {
        return other.gameObject.layer == LayerMask.NameToLayer("Placeable Collider");
    }

    protected bool OtherIsPlaceableObjectsCollider(Collider other)
    {
        return other.gameObject.layer == LayerMask.NameToLayer("Placeable Objects Collider");
    }
}
