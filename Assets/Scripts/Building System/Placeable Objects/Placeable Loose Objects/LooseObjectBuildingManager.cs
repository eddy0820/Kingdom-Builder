using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseObjectBuildingManager : AbstractPlaceableObjectBuildingManager
{
    float looseObjectEulerY;
    public float LooseObjectEulerY => looseObjectEulerY;
    bool looseObjectRotate = false;

    protected override void OnAwake() {}

    private void Update()
    {
        HandleLooseObjectRotation();
    }

    public override bool PlaceObject(out PlaceableObject placeableObject)
    {
        if(CanPlaceObject(out Vector3 pos))
        {
            placeableObject = Instantiate(GridBuildingManager.CurrentPlaceableObjectSO.Prefab, pos, Quaternion.Euler(0, looseObjectEulerY, 0)).GetComponent<PlaceableObject>();
            return true;
        }

        placeableObject = null;
        return false;
    }

    public override void Demolish(PlaceableObject placeableObject)
    {
        placeableObject.DestroySelf();
        GridBuildingManager.BuildingGhost.RefreshVisual();
    }

    public override void Rotate()
    {
        looseObjectRotate = true;
    }

    private bool CanPlaceObject(out Vector3 pos)
    {   
        pos = Mouse3D.Instance.GetMouseWorldPosition(GridBuildingManager.PlaceableObjectsColliderLayerMask);

        if(pos != Vector3.zero)
        {
            if(!GridBuildingManager.BuildingGhost.LooseObjectBuildingGhost.IsFakeGhostCollidingWithAnything())
            {
                return true;
            }
        }   

        return false;
    }

    public override bool CanPlace()
    {
        return CanPlaceObject(out Vector3 pos);
    }

    public void SetRotateFalse()
    {
        if(GridBuildingManager.CurrentPlaceableObjectSO is PlaceableLooseObjectSO) looseObjectRotate = false;
    }

    private void HandleLooseObjectRotation()
    {
        if(PlayerController.Instance.BuildModeEnabled && !PlayerController.Instance.UICanvas.BuildMenuEnabled && looseObjectRotate)
        {
            if(GridBuildingManager.CurrentPlaceableObjectSO is PlaceableLooseObjectSO)
            {
                looseObjectEulerY += Time.deltaTime * 90f;
            }
        }
    }
}
