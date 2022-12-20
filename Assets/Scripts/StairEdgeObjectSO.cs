using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stair Edge Object", menuName = "Building System/Stair Edge Object")]
public class StairEdgeObjectSO : PlaceableObjectSO
{
    protected override void SetObjectType()
    {
        objectType = PlaceableObjectTypes.StairEdgeObject;
    }
}
