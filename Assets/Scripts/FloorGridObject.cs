using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGridObject : GridObject 
{
    public enum Edge 
    {
        Up,
        Down,
        Left,
        Right
    }

    [SerializeField] private EdgePosition upFloorEdgePosition;
    [SerializeField] private EdgePosition downFloorEdgePosition;
    [SerializeField] private EdgePosition leftFloorEdgePosition;
    [SerializeField] private EdgePosition rightFloorEdgePosition;

    private EdgeObject upEdgeObject;
    private EdgeObject downEdgeObject;
    private EdgeObject leftEdgeObject;
    private EdgeObject rightEdgeObject;

    public void PlaceEdge(Edge edge, EdgeObjectSO edgeObjectSO) 
    {  
        EdgePosition edgePosition = GetEdgePosition(edge);

        Transform edgeObjectTransform = Instantiate(edgeObjectSO.Prefab, edgePosition.transform.position, edgePosition.transform.rotation);

        EdgeObject edgeObject = edgeObjectTransform.GetComponent<EdgeObject>();
        edgeObject.SetParentGridObject(this, edge);
        SetEdgeObject(edge, edgeObject); 
    }

    private EdgePosition GetEdgePosition(Edge edge) 
    {
        switch (edge) 
        {
            default:
            case Edge.Up:       return upFloorEdgePosition;
            case Edge.Down:     return downFloorEdgePosition;
            case Edge.Left:     return leftFloorEdgePosition;
            case Edge.Right:    return rightFloorEdgePosition;
        }
    }

    private void SetEdgeObject(Edge edge, EdgeObject edgeObject) 
    {
        switch (edge) 
        {
            default:
            case Edge.Up:
                upEdgeObject = edgeObject;
                break;
            case Edge.Down:
                downEdgeObject = edgeObject;
                break;
            case Edge.Left:
                leftEdgeObject = edgeObject;
                break;
            case Edge.Right:
                rightEdgeObject = edgeObject;
                break;
        }
    }

    public EdgeObject GetEdgeObject(Edge edge) 
    {
        switch (edge) 
        {
            default:
            case Edge.Up:
                return upEdgeObject;
            case Edge.Down:
                return downEdgeObject;
            case Edge.Left:
                return leftEdgeObject;
            case Edge.Right:
                return rightEdgeObject;
        }
    }

    public void DestroyEdge(Edge edge)
    {
        switch(edge)
        {
            default:
            case Edge.Up:
                Destroy(upEdgeObject.gameObject);
                break;
            case Edge.Down:
                Destroy(downEdgeObject.gameObject);
                break;
            case Edge.Left:
                Destroy(leftEdgeObject.gameObject);
                break;
            case Edge.Right:
                Destroy(rightEdgeObject.gameObject);
                break;
        }
    }


    public override void DestroySelf() 
    {
        if (upEdgeObject != null) Destroy(upEdgeObject.gameObject);
        if (downEdgeObject != null) Destroy(downEdgeObject.gameObject);
        if (leftEdgeObject != null) Destroy(leftEdgeObject.gameObject);
        if (rightEdgeObject != null) Destroy(rightEdgeObject.gameObject);

        base.DestroySelf();
    }
}
