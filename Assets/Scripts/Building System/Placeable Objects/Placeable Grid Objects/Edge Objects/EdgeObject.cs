using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class EdgeObject : PlaceableObject
{
    [SerializeField] EdgeObjectParent primaryParent;
    public EdgeObjectParent PrimaryParent => primaryParent;
    [SerializeField] EdgeObjectParent secondaryParent;
    public EdgeObjectParent SecondaryParent => secondaryParent;

    protected override PlaceableObjectTypes GetObjectType()
    {
        return PlaceableObjectTypes.EdgeObject;
    }

    public override void DestroySelf()
    {
        if(primaryParent.PrimaryParentGridObject != null) primaryParent.PrimaryParentGridObject.DestroyEdge(primaryParent.PrimaryGridEdge);
        if(secondaryParent.SecondaryParentGridObject != null) secondaryParent.SecondaryParentGridObject.DestroyEdge(secondaryParent.SecondaryGridEdge);
        
        Destroy(gameObject);
    }
}
