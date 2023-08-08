using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgePositionCollider : MonoBehaviour
{
    IHasEdges parentIHasEdgesObject;
    EdgePosition edgePosition;

    private void Awake()
    {
        parentIHasEdgesObject = GetComponentInParent<IHasEdges>();
        edgePosition = GetComponentInParent<EdgePosition>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!GridBuildingUtil.IsThisABuildingGhost(gameObject) && other.TryGetComponent(out EdgeObjectParentHolder otherEdgeObjectParentHolder))
        {
            EdgeObject otherEdgeObject = otherEdgeObjectParentHolder.GetComponentInParent<EdgeObject>();

            otherEdgeObject.SetIHasEdgesObject(otherEdgeObjectParentHolder, parentIHasEdgesObject);
            parentIHasEdgesObject.SetEdgeObject(edgePosition.Edge, otherEdgeObject);
        }
    }
}
