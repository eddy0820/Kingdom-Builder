using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgePosition : MonoBehaviour 
{
    public FloorGridObject.Edge edge;

    BoxCollider boxCollider;
    int layerMask;
    Vector3 halfExtends;

    private void OnEnable()
    {
        boxCollider = GetComponent<BoxCollider>();

        layerMask = 1 << LayerMask.NameToLayer("Wall");

        halfExtends = new Vector3((boxCollider.size.x/2) - 0.4f, boxCollider.size.y/2, boxCollider.size.z/2);

        /*Collider[] colliders = Physics.OverlapBox(transform.position + boxCollider.center - GetPositionOffset(), halfExtends, transform.rotation, layerMask);
        
        foreach(Collider collider in colliders)
        {
            if(GetComponentInParent<FloorGridObject>().GetEdgeObject(edge) != collider.GetComponentInParent<EdgeObject>())
            {
                Debug.Log("bruh");
                GetComponentInParent<FloorGridObject>().SetEdgeObject(edge, collider.GetComponentInParent<EdgeObject>());
                collider.GetComponentInParent<EdgeObject>().SetSecondaryParentGridObject(GetComponentInParent<FloorGridObject>(), edge);
            }
        }*/
    }

    private void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position + boxCollider.center - GetPositionOffset(), halfExtends, transform.rotation, layerMask);
        
        foreach(Collider collider in colliders)
        {
            if(GetComponentInParent<FloorGridObject>().GetEdgeObject(edge) != collider.GetComponentInParent<EdgeObject>())
            {
                GetComponentInParent<FloorGridObject>().SetEdgeObject(edge, collider.GetComponentInParent<EdgeObject>());
                collider.GetComponentInParent<EdgeObject>().SetSecondaryParentGridObject(GetComponentInParent<FloorGridObject>(), edge);
            }
        }
    }

    private Vector3 GetPositionOffset()
    {
        Vector3 positionOffset = Vector3.zero;

        Direction parentGridObjectDirection = GetComponentInParent<FloorGridObject>().Direction;
        
        if(edge == FloorGridObject.Edge.LeftWest || edge == FloorGridObject.Edge.LeftEast)
        {
            switch(parentGridObjectDirection)
            {
                case Direction.Up:
                    positionOffset = new Vector3(boxCollider.center.x + boxCollider.center.z, 0, boxCollider.center.z - boxCollider.center.x);
                    break;
                case Direction.Down:
                    positionOffset = new Vector3(boxCollider.center.x - boxCollider.center.z, 0, boxCollider.center.z + boxCollider.center.x);
                    break;
                case Direction.Left:
                    positionOffset = new Vector3(boxCollider.center.x * 2, 0, boxCollider.center.z * 2);
                    break;
                case Direction.Right:
                    // Nothing
                    break;
            }
        }
        else if(edge == FloorGridObject.Edge.UpWest || edge == FloorGridObject.Edge.UpEast)
        {
            switch(parentGridObjectDirection)
            {
                case Direction.Up:
                    // Nothing
                    break;
                case Direction.Down:
                    positionOffset = new Vector3(boxCollider.center.x * 2, 0, boxCollider.center.z * 2);
                    break;
                case Direction.Left:
                    positionOffset = new Vector3(boxCollider.center.x + boxCollider.center.z, 0, boxCollider.center.z - boxCollider.center.x);
                    break;
                case Direction.Right:
                    positionOffset = new Vector3(boxCollider.center.x - boxCollider.center.z, 0, boxCollider.center.z + boxCollider.center.x);
                    break;
            }
        }
        else if(edge == FloorGridObject.Edge.RightWest || edge == FloorGridObject.Edge.RightEast)
        {
            switch(parentGridObjectDirection)
            {
                case Direction.Up:
                    positionOffset = new Vector3(boxCollider.center.x - boxCollider.center.z, 0, boxCollider.center.z + boxCollider.center.x);
                    break;
                case Direction.Down:
                    positionOffset = new Vector3(boxCollider.center.x + boxCollider.center.z, 0, boxCollider.center.z - boxCollider.center.x);
                    break;
                case Direction.Left:
                    // Nothing
                    break;
                case Direction.Right:
                    positionOffset = new Vector3(boxCollider.center.x * 2, 0, boxCollider.center.z * 2);
                    break;
            }   
        }
        else if(edge == FloorGridObject.Edge.DownWest || edge == FloorGridObject.Edge.DownEast)
        {
            switch(parentGridObjectDirection)
            {
                case Direction.Up:
                    positionOffset = new Vector3(boxCollider.center.x * 2, 0, boxCollider.center.z * 2);
                    break;
                case Direction.Down:
                    // Nothing
                    break;
                case Direction.Left:
                    positionOffset = new Vector3(boxCollider.center.x - boxCollider.center.z, 0, boxCollider.center.z + boxCollider.center.x);
                    break;
                case Direction.Right:
                    positionOffset = new Vector3(boxCollider.center.x + boxCollider.center.z, 0, boxCollider.center.z - boxCollider.center.x);
                    break;
            }
        }

        return positionOffset;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Matrix4x4 prevMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix; 

        Vector3 positionOffset = GetPositionOffset();

        Vector3 pos = transform.position + boxCollider.center - positionOffset;
        pos = transform.InverseTransformPoint(pos);
        Gizmos.DrawWireCube(pos, boxCollider.size);
        Gizmos.matrix = prevMatrix;
    }
}
