using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgePositionCollider : MonoBehaviour
{
    FloorGridObject parentFloorGridObject;
    EdgePosition edgePosition;

    private void Awake()
    {
        parentFloorGridObject = GetComponentInParent<FloorGridObject>();
        edgePosition = GetComponentInParent<EdgePosition>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(!IsThisABuildingGhost() && other.TryGetComponent(out EdgeObjectParent edgeObjectParent))
        {
            edgeObjectParent.SetEdgeObjectParentFloor(parentFloorGridObject, edgePosition.edge);
        }
    }

    private bool IsThisABuildingGhost()
    {
        return gameObject.layer == LayerMask.NameToLayer("Building Ghost");
    }
}
