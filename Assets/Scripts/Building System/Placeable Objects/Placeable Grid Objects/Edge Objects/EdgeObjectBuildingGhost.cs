using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeObjectBuildingGhost : AbstractPlaceableObjectBuildingGhost
{
    protected override void OnAwake() {}

    public override void DoVisibleGhostMovement(Transform visual)
    {
        if(Mouse3D.Instance.IsLookingAtEdgePosition(out EdgePosition edgePosition))
        {
            visual.transform.position = Vector3.Lerp(visual.transform.position, edgePosition.transform.position, Time.deltaTime * 15f);
            visual.transform.rotation = Quaternion.Lerp(visual.transform.rotation, edgePosition.transform.rotation, Time.deltaTime * 25f);
        }
        else
        {
            visual.transform.position = Vector3.Lerp(visual.transform.position, Mouse3D.Instance.GetMouseWorldPosition(), Time.deltaTime * 15f);
            visual.transform.rotation = Quaternion.Lerp(visual.transform.rotation, Quaternion.identity, Time.deltaTime * 25f);
        }
    }

    public override void DoFakeGhostMovement(Transform fakeVisual)
    {
        if(Mouse3D.Instance.IsLookingAtEdgePosition(out EdgePosition edgePosition))
        {
            fakeVisual.transform.position = edgePosition.transform.position;
            fakeVisual.transform.rotation = edgePosition.transform.rotation;
        }
        else
        {
            fakeVisual.transform.position = Mouse3D.Instance.GetMouseWorldPosition();
            fakeVisual.transform.rotation = Quaternion.identity;
        }
    }

    public override void RemoveColliderScriptFromVisibleGhost()
    {
        Destroy(BuildingGhost.Visual.GetComponent<EdgeObjectColliderVisual>());
    }

    public bool IsFakeGhostCollidingWithEdgeObjectVisual()
    {
        return BuildingGhost.FakeVisual.GetComponent<EdgeObjectColliderVisual>().IsCollidingWithEdgeObjectVisual;
    }

    public void FlipEdgeObjectGhost()
    {
        if(BuildingGhost.Visual != null)
        {
            BuildingGhost.Visual.GetComponentInChildren<EdgeObjectOffset>().ChangeOffset();
        }
    }
}
