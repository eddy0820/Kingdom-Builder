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

    [Space(10)]

    [SerializeField] GameObject defaultStairEdgeLeft;
    [SerializeField] GameObject defaultStairEdgeRight;
    [SerializeField] GameObject flippedStairEdgeLeft;
    [SerializeField] GameObject flippedStairEdgeRight;

    [Space(10)]
    [SerializeField] Transform defaultCenterPivot;
    [SerializeField] Transform flippedCenterPivot;

    public override void ChangeOffset(bool offsetMode)
    {
        base.ChangeOffset(offsetMode);

        if(!offsetMode)
        {
            try
            {
                defaultPlaceableCollider.SetActive(true);
                flippedPlaceableCollider.SetActive(false);

                defaultStairEdgeLeft.SetActive(true);
                defaultStairEdgeRight.SetActive(true);
                flippedStairEdgeLeft.SetActive(false);
                flippedStairEdgeRight.SetActive(false);

                GetComponentInParent<StairObject>().SetCenterPivot(defaultCenterPivot);
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

                defaultStairEdgeLeft.SetActive(false);
                defaultStairEdgeRight.SetActive(false);
                flippedStairEdgeLeft.SetActive(true);
                flippedStairEdgeRight.SetActive(true);

                GetComponentInParent<StairObject>().SetCenterPivot(flippedCenterPivot);
            }
            catch{}
            

            visualCollider.center = flippedVisualColliderCenter;   
        }
    }
}