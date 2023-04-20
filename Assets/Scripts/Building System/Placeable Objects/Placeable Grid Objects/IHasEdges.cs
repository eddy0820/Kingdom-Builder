using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasEdges
{
    public void PlaceEdge(Edge edge, EdgeObjectSO edgeObjectSO);
    public EdgePosition GetEdgePosition(Edge edge);
    public void DestroyEdge(Edge edge);
}
