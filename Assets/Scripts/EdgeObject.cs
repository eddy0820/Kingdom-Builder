using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeObject : PlaceableObject
{
    FloorGridObject parentGridObject;
    public FloorGridObject ParentGridObject => parentGridObject;
    [ReadOnly, SerializeField] FloorGridObject.Edge gridEdge;
    public FloorGridObject.Edge GridEdge => gridEdge;

    public void SetParentGridObject(FloorGridObject gridObject, FloorGridObject.Edge edge)
    {
        parentGridObject = gridObject;
        gridEdge = edge;
    }

    public override void DestroySelf()
    {
        parentGridObject.DestroyEdge(gridEdge);
        Destroy(gameObject);
    }
}
