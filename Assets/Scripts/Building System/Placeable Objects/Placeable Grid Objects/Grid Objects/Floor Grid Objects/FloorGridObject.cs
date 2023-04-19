using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGridObject : GridObject 
{
    [SerializeField] EdgePosition upWestFloorEdgePosition;
    [SerializeField] EdgePosition upEastFloorEdgePosition;
    [SerializeField] EdgePosition downWestFloorEdgePosition;
    [SerializeField] EdgePosition downEastFloorEdgePosition;
    [SerializeField] EdgePosition leftWestFloorEdgePosition;
    [SerializeField] EdgePosition leftEastFloorEdgePosition;
    [SerializeField] EdgePosition rightWestFloorEdgePosition;
    [SerializeField] EdgePosition rightEastFloorEdgePosition;

    EdgeObject upWestEdgeObject;
    EdgeObject upEastEdgeObject;
    EdgeObject downWestEdgeObject;
    EdgeObject downEastEdgeObject;
    EdgeObject leftWestEdgeObject;
    EdgeObject leftEastEdgeObject;
    EdgeObject rightWestEdgeObject;
    EdgeObject rightEastEdgeObject;

    public void PlaceEdge(Edge edge, EdgeObjectSO edgeObjectSO) 
    {  
        EdgePosition edgePosition = GetEdgePosition(edge);

        Transform edgeObjectTransform = Instantiate(edgeObjectSO.Prefab, edgePosition.transform.position, edgePosition.transform.rotation);

        EdgeObjectOffset edgeObjectOffset = edgeObjectTransform.GetComponentInChildren<EdgeObjectOffset>();
        edgeObjectOffset.ChangeOffset();

        EdgeObject edgeObject = edgeObjectTransform.GetComponent<EdgeObject>();
    }

    public bool IsWestEdge(Edge edge)
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
    private EdgePosition GetEdgePosition(Edge edge) 
    {
        switch(edge) 
        {
            default:
            case Edge.UpWest:       return upWestFloorEdgePosition;
            case Edge.UpEast:       return upEastFloorEdgePosition;
            case Edge.DownWest:     return downWestFloorEdgePosition;
            case Edge.DownEast:     return downEastFloorEdgePosition;
            case Edge.LeftWest:     return leftWestFloorEdgePosition;
            case Edge.LeftEast:     return leftEastFloorEdgePosition;
            case Edge.RightWest:    return rightWestFloorEdgePosition;
            case Edge.RightEast:    return rightEastFloorEdgePosition;
        }
    }

    public void SetEdgeObject(Edge edge, EdgeObject edgeObject) 
    {
        switch(edge) 
        {
            default:
            case Edge.UpWest:       upWestEdgeObject = edgeObject; break;
            case Edge.UpEast:       upEastEdgeObject = edgeObject; break;
            case Edge.DownWest:     downWestEdgeObject = edgeObject; break;
            case Edge.DownEast:     downEastEdgeObject = edgeObject; break;
            case Edge.LeftWest:     leftWestEdgeObject = edgeObject; break;
            case Edge.LeftEast:     leftEastEdgeObject = edgeObject; break;
            case Edge.RightWest:    rightWestEdgeObject = edgeObject; break;
            case Edge.RightEast:    rightEastEdgeObject = edgeObject; break;
        }
    }

    public EdgeObject GetEdgeObject(Edge edge) 
    {
        switch(edge) 
        {
            default:
            case Edge.UpWest:       return upWestEdgeObject;
            case Edge.UpEast:       return upEastEdgeObject;
            case Edge.DownWest:     return downWestEdgeObject;
            case Edge.DownEast:     return downEastEdgeObject;
            case Edge.LeftWest:     return leftWestEdgeObject;
            case Edge.LeftEast:     return leftEastEdgeObject;
            case Edge.RightWest:    return rightWestEdgeObject;
            case Edge.RightEast:    return rightEastEdgeObject;
        }
    }

    public Edge GetComplimentaryEdge(Edge edge)
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

    public void DestroyEdge(Edge edge)
    {
        switch(edge)
        {
            default:
            case Edge.UpWest:       Destroy(upWestEdgeObject.gameObject); break;
            case Edge.UpEast:       Destroy(upEastEdgeObject.gameObject); break;
            case Edge.DownWest:     Destroy(downWestEdgeObject.gameObject); break;
            case Edge.DownEast:     Destroy(downEastEdgeObject.gameObject); break;
            case Edge.LeftWest:     Destroy(leftWestEdgeObject.gameObject); break;
            case Edge.LeftEast:     Destroy(leftEastEdgeObject.gameObject); break;
            case Edge.RightWest:    Destroy(rightWestEdgeObject.gameObject); break;
            case Edge.RightEast:    Destroy(rightEastEdgeObject.gameObject); break; 
        }
    }


    public override void DestroySelf() 
    {
        foreach(Edge edge in Enum.GetValues(typeof(Edge)))
        {
            EdgeObject edgeObject = GetEdgeObject(edge);

            if(edgeObject != null)
            {

                if(edgeObject.PrimaryParent.PrimaryParentGridObject == this)
                {
                    edgeObject.PrimaryParent.NullifyParent();
                }
                else if(edgeObject.SecondaryParent.SecondaryParentGridObject == this)
                {
                    edgeObject.SecondaryParent.NullifyParent();
                }

                if(edgeObject.PrimaryParent.PrimaryParentGridObject == null && edgeObject.SecondaryParent.SecondaryParentGridObject == null)
                {
                    Destroy(edgeObject.gameObject);
                }
            }
            
        } 

        base.DestroySelf();
    }
}
