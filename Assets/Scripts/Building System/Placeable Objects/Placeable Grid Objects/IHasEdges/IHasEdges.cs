using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasEdges 
{
    public GameObject PlaceEdge(Edge edge, EdgeObjectSO edgeObjectSO);
    
    public bool CanPlaceObjectInternal(EdgeObjectSO edgeObjectSO, EdgePosition edgePosition, out Edge edge, out string debugString);
    protected Dictionary<Edge, EdgeObject> EdgeObjectDictionary();

    public void SetEdgeObject(Edge edge, EdgeObject edgeObject)
    {
        Dictionary<Edge, EdgeObject> edgeObjectDictionary = EdgeObjectDictionary();
        edgeObjectDictionary[edge] = edgeObject;
    }

    public void NullifyChildrenThatMatch(EdgeObject edgeObject)
    {
        Dictionary<Edge, EdgeObject> edgeObjectDictionary = EdgeObjectDictionary();
        HashSet<Edge> childrenToNullify = new HashSet<Edge>();

        foreach(KeyValuePair<Edge, EdgeObject> edgeObjectDictionaryEntry in edgeObjectDictionary)
        {
            if(edgeObjectDictionaryEntry.Value == edgeObject)
            {
                childrenToNullify.Add(edgeObjectDictionaryEntry.Key);
            }
        }

        foreach(Edge childToNullifyEdge in childrenToNullify)
        {
            edgeObjectDictionary[childToNullifyEdge] = null;
        }
    }
}
