using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairEdgeObjectOffset : EdgeObjectOffset
{
    [Header("Stair Edge Object")]
    [SerializeField] BoxCollider parentHolderCollider;

    [Space(10)]

    [SerializeField] StairEdgeOffsetPreset defaultOffsetPreset;
    [SerializeField] StairEdgeOffsetPreset flippedOffsetPreset;

    public override void ChangeOffset()
    {
        base.ChangeOffset();

        bool b = GridBuildingManager.Instance.EdgeObjectBuildingManager.CurrentEdgeFlipMode;

        if(!GridBuildingUtil.IsThisABuildingGhost(gameObject))
        {
            GetCorrectParameters(b, out Vector3 position, out Vector3 colliderSize);

            parentHolderCollider.transform.localPosition = position;
            parentHolderCollider.size = colliderSize;
        }
    }

    private void GetCorrectParameters(bool b, out Vector3 position, out Vector3 colliderSize)
    {
        StairEdgeOffsetPreset stairEdgeOffsetPreset = null;

        if(!b)
            stairEdgeOffsetPreset = defaultOffsetPreset;
        else
            stairEdgeOffsetPreset = flippedOffsetPreset;

        position = stairEdgeOffsetPreset.Position;
        colliderSize = stairEdgeOffsetPreset.ColliderSize;
    }

    [System.Serializable]
    private class StairEdgeOffsetPreset
    {
        [SerializeField] Vector3 position;
        public Vector3 Position => position;

        [Space(10)]

        [SerializeField] Vector3 colliderSize;
        public Vector3 ColliderSize => colliderSize;
    }
}
