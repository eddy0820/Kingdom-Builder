using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseObjectBuildingGhost : AbstractPlaceableObjectBuildingGhost
{
    protected override void OnAwake() {}

    public override void DoVisibleGhostMovement(Transform visual)
    {
        visual.transform.position = Vector3.Lerp(visual.transform.position, Mouse3D.Instance.GetMouseWorldPosition(), Time.deltaTime * 15f);
        visual.transform.rotation = Quaternion.Lerp(visual.transform.rotation, Quaternion.Euler(0, GridBuildingManager.Instance.LooseObjectBuildingManager.LooseObjectEulerY, 0), Time.deltaTime * 25f);            
    }

    public override void DoFakeGhostMovement(Transform fakeVisual)
    {
        fakeVisual.transform.position = Mouse3D.Instance.GetMouseWorldPosition();
        fakeVisual.transform.rotation = Quaternion.Euler(0, GridBuildingManager.Instance.LooseObjectBuildingManager.LooseObjectEulerY, 0);
    }

    public override void RemoveColliderScriptFromVisibleGhost()
    {
        Destroy(BuildingGhost.Visual.GetComponent<LooseObjectColliderVisual>());
    }

    public bool IsFakeGhostCollidingWithAnything()
    {
        return BuildingGhost.FakeVisual.GetComponent<LooseObjectColliderVisual>().Colliding;
    }
}
