using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeObject : PlaceableObject
{
    FloorGridObject parentGridObject;
    FloorGridObject secondaryParentGridObject;
    [ReadOnly, SerializeField] FloorGridObject.Edge gridEdge;
    public FloorGridObject.Edge GridEdge => gridEdge;
    [ReadOnly, SerializeField] FloorGridObject.Edge secondaryGridEdge;
    public FloorGridObject.Edge SecondaryGridEdge => secondaryGridEdge;

    public void SetParentGridObject(FloorGridObject gridObject, FloorGridObject.Edge edge)
    {
        parentGridObject = gridObject;
        gridEdge = edge;
    }

    public void SetParentGridObjectSecondary(FloorGridObject gridObject, FloorGridObject.Edge edge)
    {
        secondaryParentGridObject = gridObject;
        secondaryGridEdge = edge;
    }

    public override void DestroySelf()
    {
        parentGridObject.DestroyEdge(gridEdge);

        if(secondaryParentGridObject != null)
        {
            secondaryParentGridObject.DestroyEdge(secondaryGridEdge);
        }
        
        Destroy(gameObject);
    }
}
