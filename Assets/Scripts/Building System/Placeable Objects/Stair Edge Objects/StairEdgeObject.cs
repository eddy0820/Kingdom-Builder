using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class StairEdgeObject : PlaceableObject
{
    StairObject parentStairObject;
    public StairObject ParentStairObject => parentStairObject;
    [ReadOnly, SerializeField] StairObject.StairEdge stairEdge;

    public void SetParentStairObject(StairObject stairObject, StairObject.StairEdge _stairEdge)
    {
        parentStairObject = stairObject;
        stairEdge = _stairEdge;
    }

    public override void DestroySelf()
    {
       parentStairObject.DestroyStairEdge(stairEdge);
        
        Destroy(gameObject);
    }
}
