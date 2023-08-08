using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class StairObject : EdgeObject, IHasEdges
{
    [Header("IHasEdges")]
    [SerializeField] List<EdgePosition> defaultEdgePositions;
    [ReadOnly, SerializeField] List<EdgePosition> edgePositions;

    Dictionary<Edge, EdgeObject> edgeObjectDictionary;

    protected override void OnAwake()
    {
        edgeObjectDictionary = new Dictionary<Edge, EdgeObject>();
        edgePositions = defaultEdgePositions;

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

        Transform edgeObjectTransform = Instantiate(edgeObjectSO.Prefab, edgePosition.transform.GetChild(0).position, edgePosition.transform.GetChild(0).rotation);

        EdgeObjectOffset edgeObjectOffset = edgeObjectTransform.GetComponentInChildren<EdgeObjectOffset>();
        edgeObjectOffset.ChangeOffset();
        
        EdgeObject edgeObject = edgeObjectTransform.GetComponentInChildren<EdgeObject>();
        edgeObject.SetBuildingType(edgeObjectSO.BuildingType);

        return edgeObjectTransform.gameObject;
    }

    public void SetEdgePositions(Edge leftEdge, GameObject left, Edge rightEdge, GameObject right)
    {
        EdgePosition leftEdgePosition = edgePositions.Find(x => x.Edge == leftEdge);
        leftEdgePosition = left.GetComponent<EdgePosition>();

        EdgePosition rightEdgePosition = edgePositions.Find(x => x.Edge == rightEdge);
        rightEdgePosition = right.GetComponent<EdgePosition>();
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

    public bool CanPlaceObjectInternal(EdgeObjectSO edgeObjectSO, EdgePosition edgePosition, out Edge edge, out string debugString)
    {
        edge = Edge.UpWest;
        debugString = "";

        if(edgeObjectDictionary.ContainsKey(edgePosition.Edge) && edgeObjectDictionary[edgePosition.Edge] == null && EdgeObjectBuildingManager.IsCompatibleWithEdgeObject(edgeObjectSO, BuildingType))
        {
            if(GridBuildingManager.Instance.BuildingGhost.EdgeObjectBuildingGhost.IsFakeGhostCollidingWithEdgeObjectVisual())
            {
                debugString = "Is Colliding With Other Edge Object";
                return false;
            }

            edge = edgePosition.Edge;
            debugString = "";
            return true;
        }

        debugString = "This Is Not A Stair Object";
        return false;
    }
}
