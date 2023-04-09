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

        if(CanPlaceObject(edgeObjectSO, out FloorGridObject floorGridObject, out FloorGridObject.Edge edge))
        {
            floorGridObject.PlaceEdge(edge, edgeObjectSO);
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

    private bool CanPlaceObject(EdgeObjectSO edgeObjectSO, out FloorGridObject floorGridObject, out FloorGridObject.Edge edge)
    {
        floorGridObject = null;
        edge = FloorGridObject.Edge.UpWest;
        
        if(Mouse3D.Instance.IsLookingAtEdgePosition(out EdgePosition edgePosition))
        {
            if(IsEdgePositionPartOfFloorGridObject(edgePosition, out floorGridObject))
            {
                if(floorGridObject.GetEdgeObject(edgePosition.edge) == null)
                {   
                    if(IsEdgeTaken(floorGridObject, edgePosition))
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
        }

        return false;
    }

    public override bool CanPlace()
    {
        EdgeObjectSO edgeObjectSO = (EdgeObjectSO) GridBuildingManager.CurrentPlaceableObjectSO;
        return CanPlaceObject(edgeObjectSO, out FloorGridObject floorGridObject, out FloorGridObject.Edge edge);
    }

    private bool IsEdgePositionPartOfFloorGridObject(EdgePosition edgePosition, out FloorGridObject floorGridObject)
    {
        return edgePosition.transform.parent.TryGetComponent<FloorGridObject>(out floorGridObject);
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
