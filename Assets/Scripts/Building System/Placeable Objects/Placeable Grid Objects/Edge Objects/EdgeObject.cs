using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class EdgeObject : PlaceableObject
{
    FloorGridObject parentGridObject;
    public FloorGridObject ParentGridObject => parentGridObject;
    FloorGridObject secondaryParentGridObject;
    public FloorGridObject SecondaryParentGridObject => secondaryParentGridObject;
    [ReadOnly, SerializeField] FloorGridObject.Edge gridEdge;
    [ReadOnly, SerializeField] FloorGridObject.Edge secondaryGridEdge;

    protected override PlaceableObjectTypes GetObjectType()
    {
        return PlaceableObjectTypes.EdgeObject;
    }

    public void SetParentGridObject(FloorGridObject gridObject, FloorGridObject.Edge edge)
    {
        parentGridObject = gridObject;
        gridEdge = edge;
    }

    public void SetSecondaryParentGridObject(FloorGridObject gridObject, FloorGridObject.Edge edge)
    {
        secondaryParentGridObject = gridObject;
        secondaryGridEdge = edge;
    }

    public void NullifyParent()
    {
        parentGridObject = null;
    }
    
    public void NullifySecondaryParent()
    {
        secondaryParentGridObject = null;
    }

    public override void DestroySelf()
    {
        parentGridObject.DestroyEdge(gridEdge);
        
        Destroy(gameObject);
    }
}
