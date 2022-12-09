using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseObject : PlaceableObject
{
    public override void DestroySelf()
    {
        Destroy(gameObject);
    }
}
