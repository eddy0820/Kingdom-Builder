using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairObject : EdgeObject
{
    public enum StairEdge 
    {
        Left,
        Right
    }

    [Space(15)]

    [SerializeField] StairEdgePosition leftStairEdgePosition;
    [SerializeField] StairEdgePosition rightStairEdgePosition;

    StairEdgeObject leftStairEdgeObject;
    StairEdgeObject rightStairEdgeObject;

    public void PlaceStairEdge(StairEdge stairEdge, StairEdgeObjectSO stairEdgeObjectSO) 
    {  
        StairEdgePosition stairEdgePosition = GetStairEdgePosition(stairEdge);

        Transform stairEdgeObjectTransform = Instantiate(stairEdgeObjectSO.Prefab, stairEdgePosition.transform.GetChild(0).position, stairEdgePosition.transform.GetChild(0).rotation);

        StairEdgeObject stairEdgeObject = stairEdgeObjectTransform.GetComponent<StairEdgeObject>();

        stairEdgeObject.SetParentStairObject(this, stairEdge);
        SetStairEdgeObject(stairEdge,stairEdgeObject); 
    }

    private StairEdgePosition GetStairEdgePosition(StairEdge stairEdge) 
    {
        switch(stairEdge) 
        {
            default:
            case StairEdge.Left:       return leftStairEdgePosition;
            case StairEdge.Right:      return rightStairEdgePosition; 
        }
    }

    public void SetStairEdgeObject(StairEdge stairEdge, StairEdgeObject stairEdgeObject) 
    {
        switch(stairEdge) 
        {
            default:
            case StairEdge.Left:       leftStairEdgeObject = stairEdgeObject; break;
            case StairEdge.Right:      rightStairEdgeObject = stairEdgeObject; break;
        }
    }

    public StairEdgeObject GetStairEdgeObject(StairEdge stairEdge) 
    {
        switch(stairEdge) 
        {
            default:
            case StairEdge.Left:       return leftStairEdgeObject;
            case StairEdge.Right:      return rightStairEdgeObject;
        }
    }

    public void DestroyStairEdge(StairEdge stairEdge)
    {
        switch(stairEdge)
        {
            default:
            case StairEdge.Left:       Destroy(leftStairEdgeObject.gameObject); break;
            case StairEdge.Right:      Destroy(rightStairEdgeObject.gameObject); break;
        }
    }

    public override void DestroySelf() 
    {
        if(leftStairEdgeObject != null)     Destroy(leftStairEdgeObject);
        if(rightStairEdgeObject != null)    Destroy(rightStairEdgeObject);

        base.DestroySelf();
    }

    public void SetCenterPivot(Transform trans)
    {
        centerPivot = trans;
    }
}
