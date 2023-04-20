using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class EdgeObjectParent : MonoBehaviour
{
    [SerializeField] bool isPrimary;

    IHasEdges primaryParentGridObject;
    public IHasEdges PrimaryParentGridObject => primaryParentGridObject;
    IHasEdges secondaryParentGridObject;
    public IHasEdges SecondaryParentGridObject => secondaryParentGridObject;

    [ShowIf("isPrimary")]
    [ReadOnly, SerializeField] Edge primaryGridEdge;
    public Edge PrimaryGridEdge => primaryGridEdge;

    [HideIf("isPrimary")]
    [ReadOnly, SerializeField] Edge secondaryGridEdge;
    public Edge SecondaryGridEdge => secondaryGridEdge;

    public void SetEdgeObjectParentFloor(FloorGridObject floorGridObject, Edge edge)
    {
        if(isPrimary)
        {
            primaryParentGridObject = floorGridObject;
            primaryGridEdge = edge;
            
        }
        else
        {
            secondaryParentGridObject = floorGridObject;
            secondaryGridEdge = edge;
        }

        floorGridObject.SetEdgeObject(edge, GetComponentInParent<EdgeObject>());
    }

    public void SetEdgeObjectParentStair(StairObject stairObject, Edge edge)
    {
        primaryParentGridObject = stairObject;
        primaryGridEdge = edge;
    }

    public void NullifyParent()
    {
        if(isPrimary)
        {
            primaryParentGridObject = null;
        }
        else
        {
            secondaryParentGridObject = null;
        }
    }
}
