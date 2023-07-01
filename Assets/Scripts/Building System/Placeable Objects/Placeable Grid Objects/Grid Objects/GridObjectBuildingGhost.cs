using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObjectBuildingGhost : AbstractPlaceableObjectBuildingGhost
{
    protected override void OnAwake() {}

    public override void DoVisibleGhostMovement(Transform visual)
    {
        visual.position = Vector3.Lerp(visual.position, GridBuildingManager.Instance.GridObjectBuildingManager.GetGridObjectPosition(), Time.deltaTime * 15f);
        visual.rotation = Quaternion.Lerp(visual.rotation, GridBuildingManager.Instance.GridObjectBuildingManager.GetGridObjectRotation(), Time.deltaTime * 15f);
    }

    public override void DoFakeGhostMovement(Transform fakeVisual)
    {
        fakeVisual.transform.position = GridBuildingManager.Instance.GridObjectBuildingManager.GetMouseWorldSnappedPosition();
        fakeVisual.transform.rotation = GridBuildingManager.Instance.GridObjectBuildingManager.GetGridObjectRotation();
    }

    public bool IsFakeGhostCollidingWithEdgeObject()
    {
        return BuildingGhost.FakeVisual.GetComponent<GridObjectColliderVisual>().IsCollidingWithEdgeObject;
    }
}
