using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseObject : PlaceableObject
{
    protected override PlaceableObjectTypes GetObjectType()
    {
        return PlaceableObjectTypes.LooseObject;
    }
    public override void DestroySelf()
    {
        Destroy(gameObject);
    }
}
