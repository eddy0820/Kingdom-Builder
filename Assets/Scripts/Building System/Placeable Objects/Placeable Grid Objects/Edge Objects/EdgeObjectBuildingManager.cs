using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class EdgeObjectBuildingManager : AbstractPlaceableObjectBuildingManager
{
    [ReadOnly, SerializeField] bool currentEdgeFlipMode = false;
    public bool CurrentEdgeFlipMode => currentEdgeFlipMode;

    protected override void OnAwake() {}

    public override bool PlaceObject(out PlaceableObject placeableObject)
    {
        EdgeObjectSO edgeObjectSO = (EdgeObjectSO) GridBuildingManager.CurrentPlaceableObjectSO;

        if(CanPlaceObject(edgeObjectSO, out IHasEdges iHasEdgesObject, out Edge edge, out string debugString))
        {
            placeableObject = iHasEdgesObject.PlaceEdge(edge, edgeObjectSO).GetComponent<PlaceableObject>();
            return true;
        }

        Debug.Log("Can't place Edge Object! " + debugString);
        placeableObject = null;
        return false;
    }
    
    public override void Demolish(PlaceableObject placeableObject)
    {
        placeableObject.DestroySelf();
    }

    public override void Rotate()
    {
        currentEdgeFlipMode = !currentEdgeFlipMode;

        GridBuildingManager.BuildingGhost.EdgeObjectBuildingGhost.FlipEdgeObjectGhost();
    }  

    private bool CanPlaceObject(EdgeObjectSO edgeObjectSO, out IHasEdges iHasEdgesObject, out Edge edge, out string debugString)
    {
        iHasEdgesObject = null;
        edge = Edge.UpWest;
        
        if(Mouse3D.Instance.IsLookingAtEdgePosition(out EdgePosition edgePosition))
        {
            edgePosition.transform.parent.TryGetComponent<IHasEdges>(out iHasEdgesObject);

            return iHasEdgesObject.CanPlaceObjectInternal(edgeObjectSO, edgePosition, out edge, out debugString);
        }

        debugString = "Not Looking at Edge Position";
        return false;
    }

    public override bool CanPlace()
    {
        EdgeObjectSO edgeObjectSO = (EdgeObjectSO) GridBuildingManager.CurrentPlaceableObjectSO;
        return CanPlaceObject(edgeObjectSO, out IHasEdges iHasEdgesObject, out Edge edge, out string debugString);
    }

    public bool ShouldBuildingGhostSnapToEdgePosition(out EdgePosition edgePosition)
    {
        if(!Mouse3D.Instance.IsLookingAtEdgePosition(out edgePosition)) return false;

        edgePosition.transform.parent.TryGetComponent<PlaceableObject>(out PlaceableObject placeableObject);

        return IsCompatibleWithEdgeObject((EdgeObjectSO) GridBuildingManager.CurrentPlaceableObjectSO, placeableObject.BuildingType);
    }

    public static bool IsCompatibleWithEdgeObject(EdgeObjectSO edgeObjectSO, BuildingTypes buildingTypeToCheck)
    {
        foreach(BuildingTypes buildingType in edgeObjectSO.CompatibleBuildingTypes)
        {
            if(buildingType == buildingTypeToCheck)
            {
                return true;
            }
        }

        return false;
    }
}
