using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FloorGridObject : GridObject, IHasEdges
{
    [SerializeField] List<EdgePosition> edgePositions;

    Dictionary<Edge, EdgeObject> edgeObjectDictionary;

    protected override void OnAwake()
    {
        edgeObjectDictionary = new Dictionary<Edge, EdgeObject>();

        foreach(EdgePosition edgePosition in edgePositions)
        {
            edgeObjectDictionary.Add(edgePosition.Edge, null);
        }
    }

    Dictionary<Edge, EdgeObject> IHasEdges.EdgeObjectDictionary()
    {
        return edgeObjectDictionary;
    }

    public GameObject PlaceEdge(Edge edge, EdgeObjectSO edgeObjectSO) 
    {  
        EdgePosition edgePosition = edgePositions.Find(x => x.Edge == edge);

        Transform edgeObjectTransform = Instantiate(edgeObjectSO.Prefab, edgePosition.transform.position, edgePosition.transform.rotation);

        EdgeObjectOffset edgeObjectOffset = edgeObjectTransform.GetComponentInChildren<EdgeObjectOffset>();
        edgeObjectOffset.ChangeOffset();

        EdgeObject edgeObject = edgeObjectTransform.GetComponentInChildren<EdgeObject>();
        edgeObject.SetBuildingType(edgeObjectSO.BuildingType);
        edgeObject.SetMaterialSoundType(edgeObjectSO.MaterialSoundType);

        return edgeObjectTransform.gameObject;
    }

    public bool CanPlaceObjectInternal(EdgeObjectSO edgeObjectSO, EdgePosition edgePosition, out Edge edge, out string debugString)
    {
        edge = Edge.UpWest;
        debugString = "";

        if(edgeObjectDictionary[edgePosition.Edge] == null && EdgeObjectBuildingManager.IsCompatibleWithEdgeObject(edgeObjectSO, buildingType))
        {   
            if(IsEdgeTaken(edgePosition)) //  Might not need this check, I think it's a dumbo mistake
            {
                debugString = "Edge Is Taken.";
                return false;
            }

            if(IsEdgeWidthTwo(edgeObjectSO) && 
                IsWestEdge(edgePosition.Edge) && 
                IsEastEdgeTaken(edgePosition))
            {
                debugString = "Complimentary Edge Is Taken.";
                return false;
            }

            if(GridBuildingManager.Instance.BuildingGhost.EdgeObjectBuildingGhost.IsFakeGhostCollidingWithEdgeObjectVisual())
            {
                debugString = "Is Colliding With Other Edge Object";
                return false;
            }

            edge = edgePosition.Edge;
            debugString = "";
            return true;
        }

        debugString = "This Is Not A Floor Grid Object";
        return false;
    }

    public override void DestroySelf() 
    {
        foreach(KeyValuePair<Edge, EdgeObject> edgeObjectDictionaryEntry in edgeObjectDictionary)
        {
            if(edgeObjectDictionaryEntry.Value != null)
            {
                if(edgeObjectDictionaryEntry.Value.NullifyParentsThatMatch(this))
                {
                    edgeObjectDictionary[edgeObjectDictionaryEntry.Key].DestroySelf();
                }
            }
        }

        base.DestroySelf();
    }

    public static bool IsEdgeWidthTwo(EdgeObjectSO edgeObjectSO)
    {
        return edgeObjectSO.Width == EdgeObjectSO.EdgeWidth.Two;
    }

    public bool IsEastEdgeTaken(EdgePosition edgePosition)
    {
        edgeObjectDictionary.TryGetValue(GetComplimentaryEdge(edgePosition.Edge), out EdgeObject edgeObject);
        return edgeObject != null;
    }

    public bool IsEdgeTaken(EdgePosition edgePosition)
    {
        return edgeObjectDictionary[edgePosition.Edge] != null;
    }

    public static bool IsWestEdge(Edge edge)
    {
        switch(edge) 
        {
            default:
            case Edge.UpWest:       return true;
            case Edge.UpEast:       return false;
            case Edge.DownWest:     return true;
            case Edge.DownEast:     return false;
            case Edge.LeftWest:     return true;
            case Edge.LeftEast:     return false;
            case Edge.RightWest:    return true;
            case Edge.RightEast:    return false;
        }
    }

    public static Edge GetComplimentaryEdge(Edge edge)
    {
        switch(edge) 
        {
            default:
            case Edge.UpWest:       return Edge.UpEast;
            case Edge.UpEast:       return Edge.UpWest;
            case Edge.DownWest:     return Edge.DownEast;
            case Edge.DownEast:     return Edge.DownWest;
            case Edge.LeftWest:     return Edge.LeftEast;
            case Edge.LeftEast:     return Edge.LeftWest;
            case Edge.RightWest:    return Edge.RightEast;
            case Edge.RightEast:    return Edge.RightWest;
        }
    }
}
