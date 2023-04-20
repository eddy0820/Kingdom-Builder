using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairObject : EdgeObject, IHasEdges
{
    EdgePosition currentLeftStairEdgePosition;
    EdgePosition currentRightStairEdgePosition;

    EdgeObject leftStairEdgeObject;
    EdgeObject rightStairEdgeObject;

    public void PlaceEdge(Edge edge, EdgeObjectSO edgeObjectSO)
    {
        EdgePosition edgePosition = GetEdgePosition(edge);

        Transform edgeObjectTransform = Instantiate(edgeObjectSO.Prefab, edgePosition.transform.GetChild(0).position, edgePosition.transform.GetChild(0).rotation);

        //StairObjectOffset edgeObjectOffset = edgeObjectTransform.GetComponentInChildren<StairObjectOffset>();
        //edgeObjectOffset.ChangeOffset();

        EdgeObject edgeObject = edgeObjectTransform.GetComponentInChildren<EdgeObject>();
        edgeObject.SetBuildingType(edgeObjectSO.BuildingType);

        if(edge == Edge.LeftWest)
        {
            leftStairEdgeObject = edgeObject;
        }
        else
        {
            rightStairEdgeObject = edgeObject;
        }

        edgeObject.SetPrimaryParentParentObjectStair(this, edge);
    }

    public EdgePosition GetEdgePosition(Edge edge)
    {
        switch(edge) 
        {
            default:
            case Edge.LeftWest:       return currentLeftStairEdgePosition;
            case Edge.RightWest:      return currentRightStairEdgePosition; 
        }
    }

    public EdgeObject GetEdgeObject(Edge edge) 
    {
        switch(edge) 
        {
            default:
            case Edge.LeftWest:     return leftStairEdgeObject;
            case Edge.RightWest:    return rightStairEdgeObject;
        }
    }

    public void SetEdgePositions(GameObject left, GameObject right)
    {
        currentLeftStairEdgePosition = left.GetComponent<EdgePosition>();
        currentRightStairEdgePosition = right.GetComponent<EdgePosition>();
    }

    public void DestroyEdge(Edge edge)
    {
        switch(edge)
        {
            default:
            case Edge.LeftWest:       Destroy(leftStairEdgeObject.gameObject); break;
            case Edge.RightWest:      Destroy(rightStairEdgeObject.gameObject); break;
        }
    }

    public override void DestroySelf() 
    {
        if(leftStairEdgeObject != null)     Destroy(leftStairEdgeObject.gameObject);
        if(rightStairEdgeObject != null)    Destroy(rightStairEdgeObject.gameObject);

        base.DestroySelf();
    }

    public void SetCenterPivot(Transform trans)
    {
        centerPivot = trans;
    }
}
