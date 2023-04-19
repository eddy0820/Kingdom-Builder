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

    StairObject parentStairObject;

    private void OnEnable()
    {
        if(!IsThisABuildingGhost())
        {
            parentStairObject = GetComponentInParent<StairObject>();
        }
    }

    public override void ChangeOffset()
    {
        base.ChangeOffset();

        if(!GridBuildingManager.Instance.EdgeObjectBuildingManager.CurrentEdgeFlipMode)
        {
            if(!IsThisABuildingGhost())
            {
                defaultPlaceableCollider.SetActive(true);
                flippedPlaceableCollider.SetActive(false);

                defaultStairEdgeLeft.SetActive(true);
                defaultStairEdgeRight.SetActive(true);
                flippedStairEdgeLeft.SetActive(false);
                flippedStairEdgeRight.SetActive(false);

                parentStairObject.SetStairEdgePositions(defaultStairEdgeLeft, defaultStairEdgeRight);
                parentStairObject.SetCenterPivot(defaultCenterPivot);
            }

            visualCollider.center = defaultVisualColliderCenter; 
        }
        else
        {
            if(!IsThisABuildingGhost())
            {
                defaultPlaceableCollider.SetActive(false);
                flippedPlaceableCollider.SetActive(true);

                defaultStairEdgeLeft.SetActive(false);
                defaultStairEdgeRight.SetActive(false);
                flippedStairEdgeLeft.SetActive(true);
                flippedStairEdgeRight.SetActive(true);

                parentStairObject.SetStairEdgePositions(flippedStairEdgeLeft, flippedStairEdgeRight);
                parentStairObject.SetCenterPivot(flippedCenterPivot);
            }
            
            visualCollider.center = flippedVisualColliderCenter; 
        }
    }

    private bool IsThisABuildingGhost()
    {
        return gameObject.layer == LayerMask.NameToLayer("Building Ghost");
    }
}
