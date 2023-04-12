using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class EdgeObjectParent : MonoBehaviour
{
    [SerializeField] bool isPrimary;

    FloorGridObject primaryParentGridObject;
    public FloorGridObject PrimaryParentGridObject => primaryParentGridObject;
    FloorGridObject secondaryParentGridObject;
    public FloorGridObject SecondaryParentGridObject => secondaryParentGridObject;

    [ShowIf("isPrimary")]
    [ReadOnly, SerializeField] Edge primaryGridEdge;
    public Edge PrimaryGridEdge => primaryGridEdge;

    [HideIf("isPrimary")]
    [ReadOnly, SerializeField] Edge secondaryGridEdge;
    public Edge SecondaryGridEdge => secondaryGridEdge;

    public void SetEdgeObjectParent(FloorGridObject floorGridObject, Edge edge)
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
