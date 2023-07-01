using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasEdges
{
    public GameObject PlaceEdge(Edge edge, EdgeObjectSO edgeObjectSO);
    public EdgePosition GetEdgePosition(Edge edge);
    public void DestroyEdge(Edge edge);
    public bool CanPlaceObjectInternal(EdgeObjectSO edgeObjectSO, EdgePosition edgePosition, out Edge edge, out string debugString);
}
