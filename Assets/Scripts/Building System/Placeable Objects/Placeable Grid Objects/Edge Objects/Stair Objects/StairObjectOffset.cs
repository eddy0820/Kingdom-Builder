using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairObjectOffset : EdgeObjectOffset
{
    [Header("Stair Object")]
    [SerializeField] BoxCollider visualCollider;

    [Space(10)]

    [SerializeField] StairOffsetPreset defaultOffsetPreset;
    [SerializeField] StairOffsetPreset flippedOffsetPreset;

    StairObject parentStairObject;

    private void OnEnable()
    {
        if(!GridBuildingUtil.IsThisABuildingGhost(gameObject))
        {
            parentStairObject = GetComponentInParent<StairObject>();
        }
    }

    public override void ChangeOffset()
    {
        base.ChangeOffset();

        bool b = GridBuildingManager.Instance.EdgeObjectBuildingManager.CurrentEdgeFlipMode;
        GetCorrectParameters(b, out EdgePosition leftEdgePosition, out EdgePosition rightEdgePosition, out Vector3 visualColliderCenter);

        if(!GridBuildingUtil.IsThisABuildingGhost(gameObject))
        {
            defaultOffsetPreset.PlaceableCollider.SetActive(!b);
            flippedOffsetPreset.PlaceableCollider.SetActive(b);

            defaultOffsetPreset.LeftEdgePosition.gameObject.SetActive(!b);
            defaultOffsetPreset.RightEdgePosition.gameObject.SetActive(!b);
            flippedOffsetPreset.LeftEdgePosition.gameObject.SetActive(b);
            flippedOffsetPreset.RightEdgePosition.gameObject.SetActive(b);

            parentStairObject.SetEdgePositions(leftEdgePosition.Edge, leftEdgePosition.gameObject, rightEdgePosition.Edge, rightEdgePosition.gameObject);
        }

        visualCollider.center = visualColliderCenter;
    }

    private void GetCorrectParameters(bool b, out EdgePosition leftEdgePosition, out EdgePosition rightEdgePosition, out Vector3 visualColliderCenter)
    {
        StairOffsetPreset stairOffsetPreset = null;
        
        if(!b) // Default
            stairOffsetPreset = defaultOffsetPreset;
        else // Flipped
            stairOffsetPreset = flippedOffsetPreset;
    
        leftEdgePosition = stairOffsetPreset.LeftEdgePosition;
        rightEdgePosition = stairOffsetPreset.RightEdgePosition;
        visualColliderCenter = stairOffsetPreset.VisualColliderCenter;
    }

    [System.Serializable]
    private class StairOffsetPreset
    {
        [SerializeField] GameObject placeableCollider;
        public GameObject PlaceableCollider => placeableCollider;
        [SerializeField] EdgePosition leftEdgePosition;
        public EdgePosition LeftEdgePosition => leftEdgePosition;
        [SerializeField] EdgePosition rightEdgePosition;
        public EdgePosition RightEdgePosition => rightEdgePosition;
        [SerializeField] Vector3 visualColliderCenter;
        public Vector3 VisualColliderCenter => visualColliderCenter; 
    }
}
