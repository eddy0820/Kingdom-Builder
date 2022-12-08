using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Edge Object", menuName = "Building System/Edge Object")]
public class EdgeObjectSO : PlaceableObjectSO
{
    protected override void SetObjectType()
    {
        objectType = PlaceableObjectTypes.EdgeObject;
    }
}
