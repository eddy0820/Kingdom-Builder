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

        if(CanPlaceObject(edgeObjectSO, out IHasEdges iHasEdgesObject, out Edge edge))
        {
            iHasEdgesObject.PlaceEdge(edge, edgeObjectSO);
        }
        else
        {
            Debug.Log("Can't place Edge Object!");
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

    private bool CanPlaceObject(EdgeObjectSO edgeObjectSO, out IHasEdges iHasEdgesObject, out Edge edge)
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
                        return false;
                    }

                    if(IsEdgeWidthTwo(edgeObjectSO) && 
                       IsTargetingWestEdge(floorGridObject, edgePosition) && 
                       IsEastEdgeTaken(floorGridObject, edgePosition))
                    {
                        return false;
                    }

                    if(GridBuildingManager.BuildingGhost.EdgeObjectBuildingGhost.IsFakeGhostCollidingWithEdgeObjectVisual())
                    {
                        return false;
                    }

                    edge = edgePosition.edge;
                    return true;
                }
            }
            else if(IsEdgePositionPartOfStairObject(edgePosition, out StairObject stairObject))
            {
                iHasEdgesObject = stairObject;

                if(stairObject.GetEdgeObject(edgePosition.edge) == null && IsCompatibleWithThisObject(edgeObjectSO, stairObject.BuildingType))
                {
                    edge = edgePosition.edge;
                    return true;
                }
            }
        }

        return false;
    }

    public override bool CanPlace()
    {
        EdgeObjectSO edgeObjectSO = (EdgeObjectSO) GridBuildingManager.CurrentPlaceableObjectSO;
        return CanPlaceObject(edgeObjectSO, out IHasEdges iHasEdgesObject, out Edge edge);
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
