using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class EdgeObjectBuildingManager : AbstractPlaceableObjectBuildingManager
{
    [ReadOnly, SerializeField] bool currentEdgeFlipMode = false;
    public bool CurrentEdgeFlipMode => currentEdgeFlipMode;

    protected override void OnAwake() {}

    public override void PlaceObject()
    {
        EdgeObjectSO edgeObjectSO = (EdgeObjectSO) GridBuildingManager.CurrentPlaceableObjectSO;

        if(CanPlaceObject(edgeObjectSO, out IHasEdges iHasEdgesObject, out Edge edge, out string debugString))
        {
            iHasEdgesObject.PlaceEdge(edge, edgeObjectSO);
        }
        else
        {
            Debug.Log("Can't place Edge Object! " + debugString);
        }
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
            if(IsEdgePositionPartOfFloorGridObject(edgePosition, out FloorGridObject floorGridObject))
            {
                iHasEdgesObject = floorGridObject;

                if(floorGridObject.GetEdgeObject(edgePosition.edge) == null && IsCompatibleWithThisObject(edgeObjectSO, floorGridObject.BuildingType))
                {   
                    if(IsEdgeTaken(floorGridObject, edgePosition)) //  Might not need this check, I think it's a dumbo mistake
                    {
                        debugString = "Edge Is Taken.";
                        return false;
                    }

                    if(IsEdgeWidthTwo(edgeObjectSO) && 
                       IsTargetingWestEdge(floorGridObject, edgePosition) && 
                       IsEastEdgeTaken(floorGridObject, edgePosition))
                    {
                        debugString = "Complimentary Edge Is Taken.";
                        return false;
                    }

                    if(GridBuildingManager.BuildingGhost.EdgeObjectBuildingGhost.IsFakeGhostCollidingWithEdgeObjectVisual())
                    {
                        debugString = "Is Colliding With Other Edge Object";
                        return false;
                    }

                    edge = edgePosition.edge;
                    debugString = "";
                    return true;
                }
            }
            else if(IsEdgePositionPartOfStairObject(edgePosition, out StairObject stairObject))
            {
                iHasEdgesObject = stairObject;

                if(stairObject.GetEdgeObject(edgePosition.edge) == null && IsCompatibleWithThisObject(edgeObjectSO, stairObject.BuildingType))
                {
                    edge = edgePosition.edge;
                    debugString = "";
                    return true;
                }
            }
        }

        debugString = "Not Looking at Edge Position";
        return false;
    }

    public override bool CanPlace()
    {
        EdgeObjectSO edgeObjectSO = (EdgeObjectSO) GridBuildingManager.CurrentPlaceableObjectSO;
        return CanPlaceObject(edgeObjectSO, out IHasEdges iHasEdgesObject, out Edge edge, out string debugString);
    }

    private bool IsCompatibleWithThisObject(EdgeObjectSO edgeObjectSO, BuildingTypes buildingTypeToCheck)
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

    public bool ShouldBuildingGhostSnapToEdgePosition(out EdgePosition edgePosition)
    {
        if(!Mouse3D.Instance.IsLookingAtEdgePosition(out edgePosition))
        {
            return false;
        }

        if(IsEdgePositionPartOfFloorGridObject(edgePosition, out FloorGridObject floorGridObject))
        {
            return IsCompatibleWithThisObject((EdgeObjectSO) GridBuildingManager.CurrentPlaceableObjectSO, floorGridObject.BuildingType);
        }
        else if(IsEdgePositionPartOfStairObject(edgePosition, out StairObject stairObject))
        {
            return IsCompatibleWithThisObject((EdgeObjectSO) GridBuildingManager.CurrentPlaceableObjectSO, stairObject.BuildingType);
        }

        return false;
    }

    private bool IsEdgePositionPartOfFloorGridObject(EdgePosition edgePosition, out FloorGridObject floorGridObject)
    {
        return edgePosition.transform.parent.TryGetComponent<FloorGridObject>(out floorGridObject);
    }

    private bool IsEdgePositionPartOfStairObject(EdgePosition edgePosition, out StairObject stairObject)
    {
        return edgePosition.transform.parent.TryGetComponent<StairObject>(out stairObject);
    }

    private bool IsEdgeWidthTwo(EdgeObjectSO edgeObjectSO)
    {
        return edgeObjectSO.Width == EdgeObjectSO.EdgeWidth.Two;
    }

    private bool IsTargetingWestEdge(FloorGridObject floorGridObject, EdgePosition edgePosition)
    {
        return floorGridObject.IsWestEdge(edgePosition.edge);
    }

    private bool IsEastEdgeTaken(FloorGridObject floorGridObject, EdgePosition edgePosition)
    {
        return floorGridObject.GetEdgeObject(floorGridObject.GetComplimentaryEdge(edgePosition.edge)) != null;
    }

    private bool IsEdgeTaken(FloorGridObject floorGridObject, EdgePosition edgePosition)
    {
        return floorGridObject.GetEdgeObject(edgePosition.edge) != null;
    }

    
}
