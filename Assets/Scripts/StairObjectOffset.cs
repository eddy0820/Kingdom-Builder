using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairObjectOffset : EdgeObjectOffset
{
    [Space(10)]

    [SerializeField] GameObject defaultPlaceableCollider;
    [SerializeField] GameObject flippedPlaceableCollider;

    [Space(10)]

    [SerializeField] BoxCollider visualCollider;
    [SerializeField] Vector3 defaultVisualColliderCenter;
    [SerializeField] Vector3 flippedVisualColliderCenter;

    public override void ChangeOffset(bool offsetMode)
    {
        base.ChangeOffset(offsetMode);

        if(!offsetMode)
        {
            try
            {
                defaultPlaceableCollider.SetActive(true);
                flippedPlaceableCollider.SetActive(false);
            }
            catch{}
            

            visualCollider.center = defaultVisualColliderCenter;    
        }
        else
        {
            try
            {
                defaultPlaceableCollider.SetActive(false);
                flippedPlaceableCollider.SetActive(true);
            }
            catch{}
            

            visualCollider.center = flippedVisualColliderCenter;   
        }
    }
}
