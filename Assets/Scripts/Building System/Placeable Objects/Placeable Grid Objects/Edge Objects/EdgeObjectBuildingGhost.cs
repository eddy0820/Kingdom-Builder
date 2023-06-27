using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeObjectBuildingGhost : AbstractPlaceableObjectBuildingGhost
{
    protected override void OnAwake() {}

    public override void DoVisibleGhostMovement(Transform visual)
    {
        if(GridBuildingManager.Instance.EdgeObjectBuildingManager.ShouldBuildingGhostSnapToEdgePosition(out EdgePosition edgePosition))
        {
            visual.transform.position = Vector3.Lerp(visual.transform.position, edgePosition.PivotTransform.position, Time.deltaTime * 15f);
            visual.transform.rotation = Quaternion.Lerp(visual.transform.rotation, edgePosition.PivotTransform.rotation, Time.deltaTime * 25f);
        }
        else
        {
            visual.transform.position = Vector3.Lerp(visual.transform.position, Mouse3D.Instance.GetMouseWorldPosition(), Time.deltaTime * 15f);
            visual.transform.rotation = Quaternion.Lerp(visual.transform.rotation, Quaternion.identity, Time.deltaTime * 25f);
        }
    }

    public override void DoFakeGhostMovement(Transform fakeVisual)
    {
        if(GridBuildingManager.Instance.EdgeObjectBuildingManager.ShouldBuildingGhostSnapToEdgePosition(out EdgePosition edgePosition))
        {
            fakeVisual.transform.position = edgePosition.PivotTransform.position;
            fakeVisual.transform.rotation = edgePosition.PivotTransform.rotation;
        }
        else
        {
            fakeVisual.transform.position = Mouse3D.Instance.GetMouseWorldPosition();
            fakeVisual.transform.rotation = Quaternion.identity;
        }
    }

    public override void RemoveColliderScriptFromVisibleGhost()
    {
        RemoveColliderScriptFromVisibleGhostRecursive(BuildingGhost.Visual.gameObject);
        //Destroy(BuildingGhost.Visual.GetComponent<EdgeObjectColliderVisual>());
    }

    private void RemoveColliderScriptFromVisibleGhostRecursive(GameObject targetGameObject)
    {
        if(targetGameObject.TryGetComponent<EdgeObjectColliderVisual>(out EdgeObjectColliderVisual edgeCollider))
        {
            Destroy(edgeCollider);
        }
        else
        {
            foreach(Transform child in targetGameObject.transform)
            {
                if(child.gameObject != targetGameObject)
                {
                    RemoveColliderScriptFromVisibleGhostRecursive(child.gameObject);
                }
            }
        }
    }

    public bool IsFakeGhostCollidingWithEdgeObjectVisual()
    {
        /*if(BuildingGhost.FakeVisual.TryGetComponent<EdgeObjectColliderVisual>(out EdgeObjectColliderVisual edgeCollider))
        {
            return edgeCollider.IsCollidingWithEdgeObjectVisual;
        }

        return false;*/

        return IsFakeGhostCollidingWithEdgeObjectVisualRecursive(BuildingGhost.FakeVisual.gameObject);
    }

    private bool IsFakeGhostCollidingWithEdgeObjectVisualRecursive(GameObject targetGameObject)
    {
        if(targetGameObject.TryGetComponent<EdgeObjectColliderVisual>(out EdgeObjectColliderVisual edgeCollider))
        {
            if(edgeCollider.IsCollidingWithEdgeObjectVisual)
            {
                return true;
            }
        }

        foreach(Transform child in targetGameObject.transform)
        {
            if(IsFakeGhostCollidingWithEdgeObjectVisualRecursive(child.gameObject))
            {
                return true;
            }
        }

        return false;
    }

    public void FlipEdgeObjectGhost()
    {
        if(BuildingGhost.Visual != null)
        {
            BuildingGhost.Visual.GetComponentInChildren<EdgeObjectOffset>().ChangeOffset();
            BuildingGhost.FakeVisual.GetComponentInChildren<EdgeObjectOffset>().ChangeOffset();
        }
    }
}
